using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Exceptions;
using Reddit.NET.Client.Models.Internal.Json;

namespace Reddit.NET.Client.Command
{
    /// <summary>
    /// Defines extensions for <see cref="CommandExecutor" />.
    /// </summary>
    public static class CommandExecutorExtensions
    {
        private static readonly Lazy<JsonSerializerOptions> s_jsonSerializerOptions = new Lazy<JsonSerializerOptions>(GetJsonSerializerOptions);

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> instance, parsing the response to an instance of type <typeparamref name="TResponse" />.
        /// </summary>
        /// <param name="executor">A <see cref="CommandExecutor" /> instance.</param>
        /// <param name="command">The command to execute.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains the response of the command execution parsed as an instance of type <typeparamref name="TResponse" />.
        /// </returns>
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
        public static async Task<TResponse> ExecuteCommandAsync<TResponse>(
            this CommandExecutor executor,
            ClientCommand command)
        {
            var response = await executor.ExecuteCommandAsync(command).ConfigureAwait(false);

            return await response
                .Content
                .ReadFromJsonAsync<TResponse>()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> instance with authentication, parsing the response to an instance of type <typeparamref name="TResponse" />.
        /// </summary>
        /// <param name="executor">A <see cref="CommandExecutor" /> instance.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="authenticator">An <see cref="IAuthenticator" /> instance used to handle authentication for the command.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The result contains the response of the command execution parsed as an instance of type <typeparamref name="TResponse" />.
        /// </returns>
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
        public static async Task<TResponse> ExecuteCommandAsync<TResponse>(
            this CommandExecutor executor,
            ClientCommand command,
            IAuthenticator authenticator)
        {
            var response = await executor.ExecuteCommandAsync(command, authenticator).ConfigureAwait(false);

            return await response
                .Content
                .ReadFromJsonAsync<TResponse>(s_jsonSerializerOptions.Value)
                .ConfigureAwait(false);
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions();

            options.Converters.Add(new ThingJsonConverterFactory());

            return options;
        }
    }
}
