using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication;
using Reddit.NET.Core.Client.Builder;

namespace Reddit.NET.Example
{
    internal class Example : IHostedService
    {
        private readonly ILogger<Example> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
                private readonly ILoggerFactory _loggerFactory;

        public Example(ILogger<Example> logger, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _loggerFactory = loggerFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("StartAsync");

            var client = RedditClientBuilder
                .New
                .WithHttpClientFactory(_httpClientFactory)
                .WithLoggerFactory(_loggerFactory)
                .WithUserRefreshTokenAuthentication(GetUserRefreshTokenAuthenticationDetails())
                // .WithUsernamePasswordAuthentication(GetUsernamePasswordAuthenticationDetails())
                // .WithClientCredentialsAuthentication(GetClientCredentialsAuthenticationDetails())
            
                .Build();

            var askReddit = client.Subreddit("askreddit");

            var askRedditDetails = await askReddit.GetDetailsAsync();

            Console.WriteLine($"Subreddit [Name = {askRedditDetails.Name}, Title = {askRedditDetails.Title}]" );

            var me = client.Me();

            var meDetails = await me.GetDetailsAsync();

            Console.WriteLine($"User [Name = {meDetails.Name}]" );
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("StopAsync");

            return Task.CompletedTask;
        }

        private UserRefreshTokenAuthenticator.AuthenticationDetails GetUserRefreshTokenAuthenticationDetails()
        {
            var clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");

            var refreshToken = PromptForValue("Refresh Token");

            return new UserRefreshTokenAuthenticator.AuthenticationDetails()
            {
                RefreshToken = refreshToken,
                ClientId = clientId,
                ClientSecret = clientSecret
            };
        }

        private UsernamePasswordAuthenticator.AuthenticationDetails GetUsernamePasswordAuthenticationDetails()
        {
            var username = Environment.GetEnvironmentVariable("REDDIT_USERNAME");
            var password = Environment.GetEnvironmentVariable("REDDIT_PASSWORD");            
            var clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");

            var code = PromptForValue("2FA Code");

            return new UsernamePasswordAuthenticator.AuthenticationDetails
            {
                Username = username,
                Password = $"{password}:{code}",
                ClientId = clientId,
                ClientSecret = clientSecret
            };
        }

        private ClientCredentialsAuthenticator.AuthenticationDetails GetClientAuthenticationDetails()
        {
            var clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");

            return new ClientCredentialsAuthenticator.AuthenticationDetails
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };
        }

        private string PromptForValue(string valueName)
        {
            Console.Write($"Please enter your {valueName}: ");

            return Console.ReadLine();
        }
    }
}