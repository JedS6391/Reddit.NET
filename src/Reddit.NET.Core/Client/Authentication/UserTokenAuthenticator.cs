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
    /// An <see cref="IAuthenticator" /> implementation that uses a token retrieved via interactive authentication.
    /// </summary>
    public sealed class UserTokenAuthenticator : AutoRefreshAuthenticator
    {        
        private readonly Token _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsernamePasswordAuthenticator" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> instance used for writing log messages.</param>
        /// <param name="commandExecutor">An <see cref="CommandExecutor" /> instance used to execute commands against reddit.</param>
        /// <param name="credentials">A <see cref="InteractiveCredentials" /> instance describing the credentials to use for authentication.</param>
        public UserTokenAuthenticator(
            ILogger<UserTokenAuthenticator> logger,
            CommandExecutor commandExecutor, 
            InteractiveCredentials credentials)
            : base(logger, commandExecutor, credentials)
        {            
            _token = credentials.Token;
        }

        /// <inheritdoc />
        protected override Task<AuthenticationContext> DoAuthenticateAsync()
        {
            return Task.FromResult<AuthenticationContext>(new UserTokenAuthenticationContext(_token)); 
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