using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Authentication;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Authentication
{
    public class UserTokenAuthenticator : AutoRefreshAuthenticator
    {        
        private readonly Token _token;

        public UserTokenAuthenticator(
            ILogger<UserTokenAuthenticator> logger,
            CommandExecutor commandExecutor, 
            InteractiveCredentials credentials)
            : base(logger, commandExecutor, credentials)
        {            
            _token = credentials.Token;
        }

        protected override Task<AuthenticationContext> DoAuthenticateAsync()
        {
            return Task.FromResult<AuthenticationContext>(new UserTokenAuthenticationContext(_token)); 
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

        public class AuthenticationDetails 
        {
            public string RefreshToken { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}