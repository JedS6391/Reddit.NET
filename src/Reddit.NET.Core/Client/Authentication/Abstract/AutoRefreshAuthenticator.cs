using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command;

namespace Reddit.NET.Core.Client.Authentication.Abstract
{
    public abstract class AutoRefreshAuthenticator : BaseAuthenticator
    {        
        private static readonly SemaphoreSlim AuthenticationContextCreationLock = new SemaphoreSlim(1, 1);        

        private AuthenticationContext _authenticationContext;
        private DateTimeOffset? _authenticationContextCreatedAt;        
        
        protected ILogger<AutoRefreshAuthenticator> Logger;
        protected Credentials Credentials;

        protected AutoRefreshAuthenticator(
            ILogger<AutoRefreshAuthenticator> logger, 
            CommandExecutor commandExecutor,
            Credentials credentials)
            : base(commandExecutor)
        {                    
            Logger = logger;
            Credentials = credentials;
        }

        protected abstract Task<AuthenticationContext> DoAuthenticateAsync();

        protected abstract Task<AuthenticationContext> DoRefreshAsync(AuthenticationContext currentContext);

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