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
        private readonly CommandFactory _commandFactory;
        private readonly Token _token;

        public UserTokenAuthenticator(
            ILogger<UserTokenAuthenticator> logger,
            CommandFactory commandFactory, 
            InteractiveCredentials credentials)
            : base(logger, credentials)
        {
            _commandFactory = commandFactory;
            _token = credentials.Token;
        }

        protected override Task<AuthenticationContext> DoAuthenticateAsync()
        {
            return Task.FromResult<AuthenticationContext>(new UserTokenAuthenticationContext(_token)); 
        }

        protected override async Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext)
        {
            var refreshTokenCommand = _commandFactory.Create<AuthenticateWithRefreshTokenCommand>();

            var result = await refreshTokenCommand.ExecuteAsync(new AuthenticateWithRefreshTokenCommand.Parameters()
            {
                RefreshToken = currentContext.Token.RefreshToken,
                ClientId = Credentials.ClientId,
                ClientSecret = Credentials.ClientSecret
            });

            return new UserTokenAuthenticationContext(result.Token);
        }

        public class AuthenticationDetails 
        {
            public string RefreshToken { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}