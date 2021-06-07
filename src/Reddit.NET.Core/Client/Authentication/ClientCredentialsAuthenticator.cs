using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Authentication;

namespace Reddit.NET.Core.Client.Authentication
{
    public class ClientCredentialsAuthenticator : AutoRefreshAuthenticator
    {
        private readonly CommandFactory _commandFactory;
        private readonly ClientCredentialsAuthenticator.AuthenticationDetails _authenticationDetails;

        public ClientCredentialsAuthenticator(
            ILogger<ClientCredentialsAuthenticator> logger,
            CommandFactory commandFactory, 
            ClientCredentialsAuthenticator.AuthenticationDetails authenticationDetails)
            : base(logger)
        {
            _commandFactory = commandFactory;
            _authenticationDetails = authenticationDetails;
        }

        protected override async Task<AuthenticationContext> DoAuthenticateAsync()
        {
            var authenticateCommand = _commandFactory.Create<AuthenticateWithClientCredentialsCommand>();

            var result = await authenticateCommand.ExecuteAsync(new AuthenticateWithClientCredentialsCommand.Parameters()
            {
                ClientId = _authenticationDetails.ClientId,
                ClientSecret = _authenticationDetails.ClientSecret
            });

            return new ClientCredentialsAuthenticationContext(result.Token); 
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

            return new ClientCredentialsAuthenticationContext(result.Token);
        }

        public class AuthenticationDetails 
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}