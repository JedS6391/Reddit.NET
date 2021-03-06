using System.Threading;
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
    /// An <see cref="IAuthenticator" /> implementation that uses the <c>password</c> grant type to authenticate.
    /// </summary>
    public sealed class UsernamePasswordAuthenticator : AutoRefreshAuthenticator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsernamePasswordAuthenticator" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> instance used for writing log messages.</param>
        /// <param name="commandExecutor">An <see cref="CommandExecutor" /> instance used to execute commands against reddit.</param>
        /// <param name="credentials">A <see cref="Credentials" /> instance describing the credentials to use for authentication.</param>
        public UsernamePasswordAuthenticator(
            ILogger<UsernamePasswordAuthenticator> logger,
            CommandExecutor commandExecutor,
            Credentials credentials)
            : base(logger, commandExecutor, credentials)
        {
        }

        /// <inheritdoc />
        protected override async Task<AuthenticationContext> DoAuthenticateAsync(CancellationToken cancellationToken)
        {
            var authenticateCommand = new AuthenticateWithUsernamePasswordCommand(new AuthenticateWithUsernamePasswordCommand.Parameters()
            {
                Username = Credentials.Username,
                Password = Credentials.Password,
                ClientId = Credentials.ClientId,
                ClientSecret = Credentials.ClientSecret
            });

            var token = await CommandExecutor.ExecuteCommandAsync<Token>(authenticateCommand, cancellationToken).ConfigureAwait(false);

            return new UsernamePasswordAuthenticationContext(token);
        }

        /// <inheritdoc />
        protected override async Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext, CancellationToken cancellationToken)
        {
            // The password grant type does not support refresh tokens, so we need to completely re-authenticate.
            // TODO: Find a better way to support 2-FA, as any provided 2-FA code will have expired by the point we need to refresh.
            return await DoAuthenticateAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
