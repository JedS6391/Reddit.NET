using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Command.RateLimiting;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Command
{
    /// <summary>
    /// Responsible for facilitating HTTP communication with reddit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All HTTP operations are encapsulated in <see cref="ClientCommand" /> instances which this class can execute.
    /// </para>
    /// <para>
    /// This design allows components that need to execute HTTP requests to be decoupled from the actual HTTP communication.
    /// </para>
    /// <para>
    /// Requests that result in transient HTTP response status codes will be retried a number of times, with an exponential back-off sleep duration.
    /// </para>
    /// <para>
    /// To remain within the reddit API rate limits, command execution will be managed to ensure that the number of requests being ,ade
    /// falls within the rate limits imposed.
    /// </para>
    /// </remarks>
    public sealed class CommandExecutor
    {
        private const int RetryCount = 3;

        private static readonly HttpStatusCode[] s_httpStatusCodesToRetry = new HttpStatusCode[]
        {
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        private static readonly Func<int, TimeSpan> s_retrySleepDurationStrategy = (retryAttempt) =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));

        private readonly ILogger<CommandExecutor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRateLimiter _rateLimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutor" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> instance used for writing log messages.</param>
        /// <param name="httpClientFactory">An <see cref="IHttpClientFactory" /> instanced used to create clients for HTTP communication.</param>
        public CommandExecutor(ILogger<CommandExecutor> logger, IHttpClientFactory httpClientFactory)
            : this(logger, httpClientFactory, new TokenBucketRateLimiter(logger, TokenBucketRateLimiterOptions.Default))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutor" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> instance used for writing log messages.</param>
        /// <param name="httpClientFactory">An <see cref="IHttpClientFactory" /> instance used to create clients for HTTP communication.</param>
        /// <param name="rateLimiter">An <see cref="IRateLimiter" /> instance used to respect request rate limits.</param>
        internal CommandExecutor(
            ILogger<CommandExecutor> logger,
            IHttpClientFactory httpClientFactory,
            IRateLimiter rateLimiter)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _rateLimiter = rateLimiter;
        }

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> instance.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the response of the command execution.</returns>
        /// <exception cref="RedditClientRateLimitException">
        /// Thrown when:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The reddit API returns a response with the <see cref="HttpStatusCode.TooManyRequests" /> HTTP status code.</description>
        ///     </item>
        ///     <item>
        ///         <description>The client rate limiter cannot permit the execution of a new request.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <exception cref="RedditClientApiException">
        /// Thrown when the reddit API returns a response with the <see cref="HttpStatusCode.BadRequest" /> HTTP status code and the response
        /// body contains an error object with details of the issue.
        /// </exception>
        /// <exception cref="RedditClientResponseException">
        /// Thrown when the reddit API returns a non-successful status code that the client does not have any specific exception for.
        /// </exception>
        public async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command)
        {
            _logger.LogDebug("Executing '{CommandId}' command", command.Id);

            return await ExecuteRequestAsync(() => command.BuildRequest()).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> instance with authentication.
        /// </summary>
        /// <remarks>
        /// The command will be validated to determine whether it can be executed in the <see cref="AuthenticationContext" /> provided
        /// by the supplied <see cref="IAuthenticator" /> instance.
        ///
        /// If the command can execute in the available context, an <c>Authorization</c> header will be added to the request <paramref name="command" /> describes.
        /// </remarks>
        /// <param name="command">The command to execute.</param>
        /// <param name="authenticator">An <see cref="IAuthenticator" /> instance used to handle authentication for the command.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the response of the command execution.</returns>
        /// <exception cref="CommandNotSupportedException">Thrown when the command cannot be executed in the available <see cref="AuthenticationContext" />.</exception>
        /// <exception cref="RedditClientRateLimitException">
        /// Thrown when:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The reddit API returns a response with the <see cref="HttpStatusCode.TooManyRequests" /> HTTP status code.</description>
        ///     </item>
        ///     <item>
        ///         <description>The client rate limiter cannot permit the execution of a new request.</description>
        ///     </item>
        /// </list>
        /// </exception>
        /// <exception cref="RedditClientApiException">
        /// Thrown when the reddit API returns a response with the <see cref="HttpStatusCode.BadRequest" /> HTTP status code and the response
        /// body contains an error object with details of the issue.
        /// </exception>
        /// <exception cref="RedditClientResponseException">
        /// Thrown when the reddit API returns a non-successful status code that the client does not have any specific exception for.
        /// </exception>
        public async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command, IAuthenticator authenticator)
        {
            var authenticationContext = await authenticator.GetAuthenticationContextAsync().ConfigureAwait(false);

            if (!authenticationContext.CanExecute(command))
            {
                _logger.LogError("'{CommandId}' not supported with the configured authentication scheme ('{AuthenticationContextId}').", command.Id, authenticationContext.Id);

                throw new CommandNotSupportedException(
                    $"'{command.Id}' not supported with the configured authentication scheme ('{authenticationContext.Id}')",
                    command.Id);
            }

            _logger.LogDebug("Executing '{CommandId}' command with authentication context '{AuthenticationContextId}'.", command.Id, authenticationContext.Id);

            return await ExecuteRequestAsync(() =>
            {
                var request = command.BuildRequest();

                AddAuthorizationHeader(request, authenticationContext);

                return request;
            })
            .ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> ExecuteRequestAsync(Func<HttpRequestMessage> requestFunc)
        {
            using var lease = await _rateLimiter.AcquireAsync();

            if (!lease.IsAcquired)
            {
                throw new RedditClientRateLimitException("Failed to acquire lease to execute request.");
            }

            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);

            var retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => s_httpStatusCodesToRetry.Contains(r.StatusCode))
                .WaitAndRetryAsync(RetryCount, s_retrySleepDurationStrategy);

            var response = await retryPolicy.ExecuteAsync(async () =>
            {
                var request = requestFunc.Invoke();

                _logger.LogDebug("Executing {Method} request to '{Uri}'", request.Method, request.RequestUri);

                return await client
                    .SendAsync(request)
                    .ConfigureAwait(false);
            });

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            _logger.LogError(
                "{Method} request to '{Uri}' failed with status code '{StatusCode}'",
                response.RequestMessage.Method,
                response.RequestMessage.RequestUri,
                response.StatusCode);

            // Try to handle the failed request.
            return await HandleFailedRequestAsync(response);
        }

        private static void AddAuthorizationHeader(HttpRequestMessage request, AuthenticationContext authenticationContext) =>
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                authenticationContext.Token.AccessToken);

        private async Task<HttpResponseMessage> HandleFailedRequestAsync(HttpResponseMessage response) =>
            response.StatusCode switch
            {
                HttpStatusCode.TooManyRequests =>
                    throw new RedditClientRateLimitException($"Rate limit has been met for '{response.RequestMessage.RequestUri}' endpoint."),

                HttpStatusCode.BadRequest => await HandleBadRequestAsync(response),

                // No specific handling for this response code.
                _ => throw new RedditClientResponseException($"Request to '{response.RequestMessage.RequestUri}' endpoint failed.", response.StatusCode)
            };

        private async Task<HttpResponseMessage> HandleBadRequestAsync(HttpResponseMessage response)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<Error>();

                throw new RedditClientApiException(
                    message: $"Request to '{response.RequestMessage.RequestUri}' endpoint failed.",
                    details: new ErrorDetails(
                        type: error.Reason,
                        message: error.Explanation,
                        fields: error.Fields));
            }
            catch (JsonException jsonException)
            {
                _logger.LogError("Failed to read error details from response.", jsonException);

                // Failed to read error details from response.
                throw new RedditClientResponseException($"Request to '{response.RequestMessage.RequestUri}' endpoint failed.", response.StatusCode);
            }
        }
    }
}
