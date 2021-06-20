using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        public RedditService(
            IOptions<RedditOptions> optionsAccessor,
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory)
        {
            _options = optionsAccessor.Value;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _loggerFactory = loggerFactory;
        }

        public Uri GenerateAuthorizationUri()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var state = GetRandomState();

            // Save the state so we can validate it upon authorization completion.
            httpContext.Session.SetString(StateSessionKeyName, state);

            var credentialsBuilder = CredentialsBuilder
                .New
                .WebApp(
                    _options.ClientId,
                    _options.ClientSecret,
                    _options.RedirectUri,
                    state);

            return credentialsBuilder.AuthorizationUri;
        }

        public async Task CompleteAuthorizationAsync(string state, string code)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var storedState = httpContext.Session.GetString(StateSessionKeyName);

            if (string.IsNullOrEmpty(storedState) || storedState != state)
            {
                // TODO: Exception type
                throw new Exception("Invalid state parameter");
            }

            var client = await RedditClientBuilder
                .New
                .WithHttpClientFactory(_httpClientFactory)
                .WithLoggerFactory(_loggerFactory)
                .WithCredentialsConfiguration(credentialsBuilder =>
                {
                    var interactiveCredentialsBuilder = credentialsBuilder.WebApp(
                        _options.ClientId,
                        _options.ClientSecret,
                        _options.RedirectUri,
                        state);

                    interactiveCredentialsBuilder.Authorize(code);
                })
                .BuildAsync();

            // TODO: Need a way to let further requests after authentication obtain a client instance.
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