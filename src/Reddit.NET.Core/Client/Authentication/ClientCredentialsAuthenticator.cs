using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Authentication;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Authentication
{
    public class ClientCredentialsAuthenticator : AutoRefreshAuthenticator
    {        
        public ClientCredentialsAuthenticator(
            ILogger<ClientCredentialsAuthenticator> logger,
            CommandExecutor commandExecutor, 
            Credentials credentials)
            : base(logger, commandExecutor, credentials)
        {          
        }

        protected override async Task<AuthenticationContext> DoAuthenticateAsync()
        {
            var authenticateCommand = new AuthenticateWithClientCredentialsCommand(new AuthenticateWithClientCredentialsCommand.Parameters()
            {                
                ClientId = Credentials.ClientId,
                ClientSecret = Credentials.ClientSecret
            });

            var token = await ExecuteCommandAsync<Token>(authenticateCommand).ConfigureAwait(false);

            return new ClientCredentialsAuthenticationContext(token);
        }

        protected override async Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext)
        {
            var refreshTokenCommand = new AuthenticateWithRefreshTokenCommand(new AuthenticateWithRefreshTokenCommand.Parameters()
            {
                RefreshToken = currentContext.Token.RefreshToken,
                ClientId = Credentials.ClientId,
                ClientSecret = Credentials.ClientSecret
            });

            var token = await ExecuteCommandAsync<Token>(refreshTokenCommand).ConfigureAwait(false);

            return new UserTokenAuthenticationContext(token);
        }
    }
}