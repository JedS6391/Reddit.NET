using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Authentication.Context;
using Reddit.NET.Core.Client.Authentication.Credential;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Authentication;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Authentication
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

        /// <inheritdoc />
        protected override async Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext)
        {
            // The password grant type does not support refresh tokens, so we need to completely re-authenticate.        
            // TODO: Find a better way to support 2-FA, as any provided 2-FA code will have expired by the point we need to refresh.
            return await DoAuthenticateAsync().ConfigureAwait(false);
        }
    }
}