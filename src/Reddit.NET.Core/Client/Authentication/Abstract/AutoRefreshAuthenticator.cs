using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Credential;
using Reddit.NET.Core.Client.Command;

namespace Reddit.NET.Core.Client.Authentication.Abstract
{
    /// <summary>
    /// Provides base auto-refresh functionality for <see cref="IAuthenticator" /> implementations. 
    /// </summary>
    public abstract class AutoRefreshAuthenticator : BaseAuthenticator
    {        
        private static readonly SemaphoreSlim AuthenticationContextCreationLock = new SemaphoreSlim(1, 1);        

        private AuthenticationContext _authenticationContext;
        private DateTimeOffset? _authenticationContextCreatedAt;        

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoRefreshAuthenticator" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> instance used for writing log messages.</param>
        /// <param name="commandExecutor">An <see cref="CommandExecutor" /> instance used to execute commands against reddit.</param>
        /// <param name="credentials">A <see cref="Credentials" /> instance describing the credentials to use for authentication.</param>
        protected AutoRefreshAuthenticator(
            ILogger<AutoRefreshAuthenticator> logger, 
            CommandExecutor commandExecutor,
            Credentials credentials)
            : base(commandExecutor)
        {                    
            Logger = Requires.NotNull(logger, nameof(logger));
            Credentials = Requires.NotNull(credentials, nameof(credentials));
        }

        /// <summary>
        /// Gets the <see cref="ILogger{TCategoryName}" /> instance used by the authenticator.
        /// </summary>
        protected ILogger<AutoRefreshAuthenticator> Logger;

        /// <summary>
        /// Gets the <see cref="Credentials" /> instance used by the authenticator.
        /// </summary>
        protected Credentials Credentials;

        /// <summary>
        /// Performs the initial authentication operation.
        /// </summary>
        /// <remarks>
        /// This method is called when authentication has not been previously performed (i.e. the initial authentication).
        /// </remarks>
        /// <returns>A task representing the asynchronous operation. The result contains the authentication context.</returns>
        protected abstract Task<AuthenticationContext> DoAuthenticateAsync();

        /// <summary>
        /// Performs the refresh authentication operation.
        /// </summary>
        /// <param name="currentContext">The current <see cref="AuthenticationContext" /> instance used by the authenticator.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the new authentication context.</returns>
        protected abstract Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext);


        /// <inheritdoc />
        public override async Task<AuthenticationContext> GetAuthenticationContextAsync()
        {
            if (ShouldCreateContext())
            {
                Logger.LogDebug("Creating authentication context...");

                await CreateContextAsync().ConfigureAwait(false); 
            }

            if (ShouldRefreshContext())
            {
                Logger.LogDebug("Refreshing authentication context...");

                await RefreshContextAsync().ConfigureAwait(false);
            }

            return _authenticationContext;
        }

        private bool ShouldCreateContext() => _authenticationContext == null;

        private bool ShouldRefreshContext() => 
            _authenticationContext != null && 
            _authenticationContextCreatedAt.Value.Add(TimeSpan.FromSeconds(_authenticationContext.Token.ExpiresIn)) < DateTimeOffset.UtcNow;

        private async Task CreateContextAsync()
        {
            await AuthenticationContextCreationLock.WaitAsync().ConfigureAwait(false);

            if (!ShouldCreateContext()) 
            {
                Logger.LogDebug("Authentication context created already - skipping.");

                // Context created while waiting for the lock, nothing to do.
                AuthenticationContextCreationLock.Release();            

                return;
            } 

            var context = await DoAuthenticateAsync().ConfigureAwait(false);

            _authenticationContextCreatedAt = DateTimeOffset.UtcNow;
            _authenticationContext = context;

            Logger.LogDebug("Authentication context created at {CreatedAt}.", _authenticationContextCreatedAt);

            AuthenticationContextCreationLock.Release();            
        }

        private async Task RefreshContextAsync()
        {
            await AuthenticationContextCreationLock.WaitAsync().ConfigureAwait(false);

            var context = await DoRefreshAsync(_authenticationContext).ConfigureAwait(false);

            _authenticationContextCreatedAt = DateTimeOffset.UtcNow;
            _authenticationContext = context;

            Logger.LogDebug("Authentication context refreshed at {CreatedAt}.", _authenticationContextCreatedAt);

            AuthenticationContextCreationLock.Release();  
        }
    }
}