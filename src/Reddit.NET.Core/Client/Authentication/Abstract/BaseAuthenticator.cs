using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command;

namespace Reddit.NET.Core.Client.Authentication.Abstract
{
    public abstract class BaseAuthenticator : IAuthenticator
    {        
        private readonly CommandExecutor _commandExecutor;

        protected BaseAuthenticator(CommandExecutor commandExecutor)
        {        
            _commandExecutor = commandExecutor;
        }

        public abstract Task<AuthenticationContext> GetAuthenticationContextAsync();

        internal async Task<HttpResponseMessage> ExecuteCommandAsync(ClientCommand command) =>
            await _commandExecutor.ExecuteCommandAsync(command).ConfigureAwait(false);

        internal async Task<TResponse> ExecuteCommandAsync<TResponse>(ClientCommand command)
        {
            var response = await ExecuteCommandAsync(command).ConfigureAwait(false);

            return await response
                .Content
                .ReadFromJsonAsync<TResponse>()
                .ConfigureAwait(false);
        }
    }
}