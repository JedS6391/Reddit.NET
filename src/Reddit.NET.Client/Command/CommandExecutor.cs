using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Command.Exceptions;

namespace Reddit.NET.Client.Command
{
    /// <summary>
    /// Responsible for executing HTTP communication with reddit.
    /// </summary>
    /// <remarks>
    /// All HTTP operations are encapsulated in <see cref="ClientCommand" /> instances which this executor knows
    /// how to handle. This design allows components that need to execute HTTP requests to be decoupled from
    /// the actual HTTP communication, and instead just operate in terms of commands.
    /// </remarks>
    public sealed class CommandExecutor
    {
        private readonly ILogger<CommandExecutor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutor" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> instance used for writing log messages.</param>
        /// <param name="httpClientFactory">An <see cref="IHttpClientFactory" /> instanced used to create clients for HTTP communication.</param>
        public CommandExecutor(ILogger<CommandExecutor> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Executes the provided <see cref="ClientCommand" /> instance.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the response of the command execution.</returns>
        public async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command)
        {        
            _logger.LogDebug("Executing '{CommandId}' command", command.Id);

            HttpRequestMessage request = command.BuildRequest();

            return await ExecuteRequestAsync(request).ConfigureAwait(false);
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
        public async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command, IAuthenticator authenticator)
        {
            AuthenticationContext authenticationContext = await authenticator.GetAuthenticationContextAsync().ConfigureAwait(false);

            if (!authenticationContext.CanExecute(command))
            {
                _logger.LogError("'{CommandId}' not supported with the configured authentication scheme ('{AuthenticationContextId}').", command.Id, authenticationContext.Id);
                
                throw new CommandNotSupportedException($"'{command.Id}' not supported with the configured authentication scheme ('{authenticationContext.Id}')");
            }

            _logger.LogDebug("Executing '{CommandId}' command", command.Id);

            HttpRequestMessage request = command.BuildRequest();
            
            AddAuthorizationHeader(request, authenticationContext);            

            return await ExecuteRequestAsync(request).ConfigureAwait(false);  
        }

        private async Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestMessage request)
        {
            _logger.LogDebug("Executing {Method} request to '{Uri}'", request.Method, request.RequestUri);

            AddDefaultHeaders(request);

            HttpClient client = _httpClientFactory.CreateClient();

            HttpResponseMessage response = await client
                .SendAsync(request)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return response;
        }

        private static void AddDefaultHeaders(HttpRequestMessage request) =>
            request.Headers.Add("User-Agent", Constants.Request.ClientName);

        private static void AddAuthorizationHeader(HttpRequestMessage request, AuthenticationContext authenticationContext) =>
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                authenticationContext.Token.AccessToken);
    }
}