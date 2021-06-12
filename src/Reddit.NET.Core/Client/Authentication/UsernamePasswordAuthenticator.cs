using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Authentication;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Authentication
{
    public class UsernamePasswordAuthenticator : AutoRefreshAuthenticator
    {
        public UsernamePasswordAuthenticator(
            ILogger<UsernamePasswordAuthenticator> logger,
            CommandExecutor commandExecutor, 
            Credentials credentials)
            : base(logger, commandExecutor, credentials)
        {            
        }

        protected override async Task<AuthenticationContext> DoAuthenticateAsync()
        {
            var authenticateCommand = new AuthenticateWithUsernamePasswordCommand(new AuthenticateWithUsernamePasswordCommand.Parameters()
            {
                Username = Credentials.Username,
                Password = Credentials.Password,
                ClientId = Credentials.ClientId,
                ClientSecret = Credentials.ClientSecret
            });

            var token = await ExecuteCommandAsync<Token>(authenticateCommand).ConfigureAwait(false);

            return new UsernamePasswordAuthenticationContext(token); 
        }

        protected override async Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext)
        {
            // Username + password authentication does not support refresh tokens, so we need to completely re-authenticate.        
            // TODO: Find a better way to support 2-FA, as the 2-FA code will have expired by the point we need to refresh.
            return await DoAuthenticateAsync().ConfigureAwait(false);
        }
    }
}