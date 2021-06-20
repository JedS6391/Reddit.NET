using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit.NET.Client;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Authentication.Credential;
using Reddit.NET.Client.Builder;

namespace Reddit.NET.WebApi.Services.Interfaces
{
    public class RedditService : IRedditService
    {
        private const string StateSessionKeyName = "_RedditService_AuthorizationUri_State";

        private readonly RedditOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ITokenStorage _tokenStorage;

        public RedditService(
            IOptions<RedditOptions> optionsAccessor,
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory,
            ITokenStorage tokenStorage)
        {
            _options = optionsAccessor.Value;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _loggerFactory = loggerFactory;
            _tokenStorage = tokenStorage;
        }

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

            // Save the state so we can validate it upon authorization completion.
            httpContext.Session.SetString(StateSessionKeyName, state);                    

            return interactiveCredentialsBuilder.AuthorizationUri;
        }

        public async Task<Guid> CompleteAuthorizationAsync(string state, string code)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var storedState = httpContext.Session.GetString(StateSessionKeyName);

            if (string.IsNullOrEmpty(storedState) || storedState != state)
            {
                // TODO: Exception type
                throw new Exception("Invalid state parameter");
            }

            var credentialsBuilder = CredentialsBuilder.New;            
            var interactiveCredentialsBuilder = credentialsBuilder.WebApp(
                _options.ClientId,
                _options.ClientSecret,
                _options.RedirectUri,
                state);

            interactiveCredentialsBuilder.Authorize(code);

            var credentials = await credentialsBuilder.BuildCredentialsAsync(
                _loggerFactory,
                _httpClientFactory,
                _tokenStorage);            

            return (credentials as InteractiveCredentials).SessionId;
        }

        public async Task<RedditClient> GetClientAsync(Guid sessionId)
        {
            return await RedditClientBuilder
                .New
                .WithHttpClientFactory(_httpClientFactory)
                .WithLoggerFactory(_loggerFactory)
                .WithTokenStorage(_tokenStorage)
                .WithCredentialsConfiguration(credentialsBuilder =>
                {
                    var interactiveCredentialsBuilder = credentialsBuilder.Session(
                        _options.ClientId,
                        _options.ClientSecret,
                        _options.RedirectUri,
                        sessionId);                                    
                })
                .BuildAsync();
        }

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