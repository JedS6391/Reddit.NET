using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Authentication;

namespace Reddit.NET.Core.Client.Authentication
{
    public class UserRefreshTokenAuthenticator : AutoRefreshAuthenticator
    {
        private readonly CommandFactory _commandFactory;
        private readonly UserRefreshTokenAuthenticator.AuthenticationDetails _authenticationDetails;

        public UserRefreshTokenAuthenticator(
            ILogger<UserRefreshTokenAuthenticator> logger,
            CommandFactory commandFactory, 
            UserRefreshTokenAuthenticator.AuthenticationDetails authenticationDetails)
            : base(logger)
        {
            _commandFactory = commandFactory;
            _authenticationDetails = authenticationDetails;
        }

        protected override async Task<AuthenticationContext> DoAuthenticateAsync()
        {
            var refreshTokenCommand = _commandFactory.Create<AuthenticateWithRefreshTokenCommand>();

            var result = await refreshTokenCommand.ExecuteAsync(new AuthenticateWithRefreshTokenCommand.Parameters()
            {
                RefreshToken = _authenticationDetails.RefreshToken,
                ClientId = _authenticationDetails.ClientId,
                ClientSecret = _authenticationDetails.ClientSecret
            });

            return new UserRefreshTokenAuthenticationContext(result.Token); 
        }

        protected override async Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext)
        {
            var refreshTokenCommand = _commandFactory.Create<AuthenticateWithRefreshTokenCommand>();

            var result = await refreshTokenCommand.ExecuteAsync(new AuthenticateWithRefreshTokenCommand.Parameters()
            {
                RefreshToken = currentContext.Token.RefreshToken,
                ClientId = _authenticationDetails.ClientId,
                ClientSecret = _authenticationDetails.ClientSecret
            });

            return new UserRefreshTokenAuthenticationContext(result.Token);
        }

        public class AuthenticationDetails 
        {
            public string RefreshToken { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}