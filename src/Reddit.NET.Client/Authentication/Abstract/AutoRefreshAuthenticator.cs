using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.Extensions.Logging;
using Reddit.NET.Client.Authentication.Credential;
using Reddit.NET.Client.Command;

namespace Reddit.NET.Client.Authentication.Abstract
{
    /// <summary>
    /// Provides base auto-refresh functionality for <see cref="IAuthenticator" /> implementations.
    /// </summary>
    public abstract class AutoRefreshAuthenticator : BaseAuthenticator
    {
        private static readonly SemaphoreSlim s_authenticationContextCreationLock = new SemaphoreSlim(1, 1);

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
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the authentication context.</returns>
        protected abstract Task<AuthenticationContext> DoAuthenticateAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Performs the refresh authentication operation.
        /// </summary>
        /// <param name="currentContext">The current <see cref="AuthenticationContext" /> instance used by the authenticator.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the new authentication context.</returns>
        protected abstract Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext, CancellationToken cancellationToken);


        /// <inheritdoc />
        public override async Task<AuthenticationContext> GetAuthenticationContextAsync(CancellationToken cancellationToken = default)
        {
            if (ShouldCreateContext())
            {
                Logger.LogDebug("Creating authentication context...");

                await CreateContextAsync(cancellationToken).ConfigureAwait(false);
            }

            if (ShouldRefreshContext())
            {
                Logger.LogDebug("Refreshing authentication context...");

                await RefreshContextAsync(cancellationToken).ConfigureAwait(false);
            }

            return _authenticationContext;
        }

        private bool ShouldCreateContext() => _authenticationContext == null;

        private bool ShouldRefreshContext() =>
            _authenticationContext != null &&
            _authenticationContextCreatedAt.Value.Add(TimeSpan.FromSeconds(_authenticationContext.Token.ExpiresIn)) < DateTimeOffset.UtcNow;

        private async Task CreateContextAsync(CancellationToken cancellationToken)
        {
            await s_authenticationContextCreationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            if (!ShouldCreateContext())
            {
                Logger.LogDebug("Authentication context created already - skipping.");

                // Context created while waiting for the lock, nothing to do.
                s_authenticationContextCreationLock.Release();

                return;
            }

            var context = await DoAuthenticateAsync(cancellationToken).ConfigureAwait(false);

            _authenticationContextCreatedAt = DateTimeOffset.UtcNow;
            _authenticationContext = context;

            Logger.LogDebug("Authentication context created at {CreatedAt}.", _authenticationContextCreatedAt);

            s_authenticationContextCreationLock.Release();
        }

        private async Task RefreshContextAsync(CancellationToken cancellationToken)
        {
            await s_authenticationContextCreationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            var context = await DoRefreshAsync(_authenticationContext, cancellationToken).ConfigureAwait(false);

            _authenticationContextCreatedAt = DateTimeOffset.UtcNow;
            _authenticationContext = context;

            Logger.LogDebug("Authentication context refreshed at {CreatedAt}.", _authenticationContextCreatedAt);

            s_authenticationContextCreationLock.Release();
        }
    }
}
