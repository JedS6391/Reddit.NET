using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Authentication.Context;
using Reddit.NET.Client.Authentication.Credential;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Command.Authentication;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication
{
    /// <summary>
    /// An <see cref="IAuthenticator" /> implementation that uses the <c>client_credentials</c> grant type to authenticate.
    /// </summary>
    public sealed class ClientCredentialsAuthenticator : AutoRefreshAuthenticator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCredentialsAuthenticator" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> instance used for writing log messages.</param>
        /// <param name="commandExecutor">An <see cref="CommandExecutor" /> instance used to execute commands against reddit.</param>
        /// <param name="credentials">A <see cref="Credentials" /> instance describing the credentials to use for authentication.</param>
        public ClientCredentialsAuthenticator(
            ILogger<ClientCredentialsAuthenticator> logger,
            CommandExecutor commandExecutor,
            Credentials credentials)
            : base(logger, commandExecutor, credentials)
        {
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
