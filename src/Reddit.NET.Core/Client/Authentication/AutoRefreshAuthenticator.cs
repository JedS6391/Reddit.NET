using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;

namespace Reddit.NET.Core.Client.Authentication
{
    public abstract class AutoRefreshAuthenticator : IAuthenticator
    {    
        private AuthenticationContext _authenticationContext;
        private DateTimeOffset? _authenticationContextCreatedAt;

        private readonly SemaphoreSlim _authenticationContextCreationLock;
        private readonly ILogger<AutoRefreshAuthenticator> _logger;

        protected AutoRefreshAuthenticator(ILogger<AutoRefreshAuthenticator> logger)
        {
            _authenticationContextCreationLock = new SemaphoreSlim(1, 1);
            _logger = logger;
        }

        protected abstract Task<AuthenticationContext> DoAuthenticateAsync();

        protected abstract Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext);

        public async Task<AuthenticationContext> AuthenticateAsync()
        {
            if (ShouldCreateContext())
            {
                _logger.LogDebug("Creating authentication context...");

                await CreateContextAsync().ConfigureAwait(false); 
            }

            if (ShouldRefreshContext())
            {
                _logger.LogDebug("Refreshing authentication context...");

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
            await _authenticationContextCreationLock.WaitAsync().ConfigureAwait(false);

            if (!ShouldCreateContext()) 
            {
                _logger.LogDebug("Authentication context created already - skipping.");

                // Context created while waiting for the lock, nothing to do.
                _authenticationContextCreationLock.Release();            

                return;
            } 

            var context = await DoAuthenticateAsync().ConfigureAwait(false);

            _authenticationContextCreatedAt = DateTimeOffset.UtcNow;
            _authenticationContext = context;

            _logger.LogDebug("Authentication context created at {CreatedAt}.", _authenticationContextCreatedAt);

            _authenticationContextCreationLock.Release();            
        }

        private async Task RefreshContextAsync()
        {
            await _authenticationContextCreationLock.WaitAsync().ConfigureAwait(false);

            var context = await DoRefreshAsync(_authenticationContext).ConfigureAwait(false);

            _authenticationContextCreatedAt = DateTimeOffset.UtcNow;
            _authenticationContext = context;

            _logger.LogDebug("Authentication context refreshed at {CreatedAt}.", _authenticationContextCreatedAt);

            _authenticationContextCreationLock.Release();  
        }
    }
}