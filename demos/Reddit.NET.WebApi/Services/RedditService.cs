using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit.NET.Client;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Builder;
using Reddit.NET.Client.Command;

namespace Reddit.NET.WebApi.Services.Interfaces
{
    /// <inheritdoc />
    public class RedditService : IRedditService
    {
        private const string StateSessionKeyName = "_RedditService_AuthorizationUri_State";

        private readonly RedditOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly CommandExecutor _commandExecutor;
        private readonly ITokenStorage _tokenStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditService" /> class.
        /// </summary>
        /// <param name="optionsAccessor"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="commandExecutor"></param>
        /// <param name="tokenStorage"></param>
        public RedditService(
            IOptions<RedditOptions> optionsAccessor,
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory,
            CommandExecutor commandExecutor,
            ITokenStorage tokenStorage)
        {
            _options = optionsAccessor.Value;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _loggerFactory = loggerFactory;
            _commandExecutor = commandExecutor;
            _tokenStorage = tokenStorage;
        }

        /// <inheritdoc />
        public Uri GenerateAuthorizationUri()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var state = GetRandomState();

            // Use the credentials builder to get a new authorization URI.
            var interactiveCredentialsBuilder = CredentialsBuilder
                .New
                .WebApp(
                    _options.ClientId,
                    _options.ClientSecret,
                    _options.RedirectUri,
                    state);

            var authorizationUri = interactiveCredentialsBuilder.AuthorizationUri;

            // Save the state so we can validate it upon authorization completion.
            httpContext.Session.SetString(StateSessionKeyName, state);

            return authorizationUri;
        }

        /// <inheritdoc />
        public async Task<Guid> CompleteAuthorizationAsync(string state, string code)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Validate the state matches what we expect for this session.
            var storedState = httpContext.Session.GetString(StateSessionKeyName);

            if (string.IsNullOrEmpty(storedState) || storedState != state)
            {
                // TODO: Exception type
                throw new Exception("Invalid state parameter");
            }

            // State matches, so remove it
            httpContext.Session.Remove(StateSessionKeyName);

            // Use the credentials builder to complete the interactive authentication.
            var interactiveCredentialsBuilder = CredentialsBuilder
                .New
                .WebApp(
                    _options.ClientId,
                    _options.ClientSecret,
                    _options.RedirectUri,
                    state);

            interactiveCredentialsBuilder.Authorize(code);

            await interactiveCredentialsBuilder.AuthenticateAsync(
                _commandExecutor,
                _tokenStorage);

            var credentials = interactiveCredentialsBuilder.Build();

            return credentials.SessionId;
        }

        /// <inheritdoc />
        public async Task<RedditClient> GetClientAsync(Guid sessionId)
        {
            return await RedditClientBuilder
                .New                
                .WithLoggerFactory(_loggerFactory)
                .WithHttpClientFactory(_httpClientFactory)
                .WithTokenStorage(_tokenStorage)
                .WithCredentialsConfiguration(credentialsBuilder =>
                {
                    // Use credentials based on the provided session.
                    credentialsBuilder.Session(
                        _options.ClientId,
                        _options.ClientSecret,
                        _options.RedirectUri,
                        sessionId);                                    
                })
                .BuildAsync();
        }

        /// <inheritdoc />  
        public async Task EndSessionAsync(Guid sessionId)
        {
            await _tokenStorage.RemoveTokenAsync(sessionId);
        }

        private static string GetRandomState() 
        {
            using var random = new RNGCryptoServiceProvider();

            byte[] tokenData = new byte[32];
            
            random.GetBytes(tokenData);

            return Convert.ToBase64String(tokenData);
        }
    }
}