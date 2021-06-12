using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Exceptions;

namespace Reddit.NET.Core.Client.Command
{
    public class CommandExecutor
    {
        private readonly ILogger<CommandExecutor> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public CommandExecutor(ILogger<CommandExecutor> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command)
        {        
            _logger.LogDebug("Executing '{CommandId}' command", command.Id);

            var request = command.BuildRequest();

            return await ExecuteRequestAsync(request).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command, IAuthenticator authenticator)
        {
            var authenticationContext = await authenticator.GetAuthenticationContextAsync().ConfigureAwait(false);

            if (!authenticationContext.CanExecute(command))
            {
                _logger.LogError("'{CommandId}' not supported with the configured authentication scheme ('{AuthenticationContextId}').", command.Id, authenticationContext.Id);
                
                throw new CommandNotSupportedException($"'{command.Id}' not supported with the configured authentication scheme ('{authenticationContext.Id}')");
            }

            _logger.LogDebug("Executing '{CommandId}' command", command.Id);

            var request = command.BuildRequest();
            
            AddAuthorizationHeader(request, authenticationContext);            

            return await ExecuteRequestAsync(request).ConfigureAwait(false);  
        }

        private async Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestMessage request)
        {
            _logger.LogDebug("Executing {Method} request to '{Uri}'", request.Method, request.RequestUri);

            AddDefaultHeaders(request);
            
            var client = _httpClientFactory.CreateClient();

            var response = await client
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