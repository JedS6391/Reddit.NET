using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Reddit.NET.Client.Builder;

namespace Reddit.NET.Client.IntegrationTests.Shared
{
    public static class TestRedditClientProvider
    {
        private static readonly Lazy<RedditClient> s_client = new Lazy<RedditClient>(BuildClient);

        public static RedditClient GetClient() => s_client.Value;

        private static RedditClient BuildClient()
        {
            var services = new ServiceCollection();

            services
                .AddLogging(builder => builder.AddDebug())
                .AddRedditHttpClient(userAgent: "macosx:Reddit.NET.Client.IntegrationTests:v0.1.0 (by JedS6391)");

            var provider = services.BuildServiceProvider();

            return RedditClientBuilder
                .New
                .WithHttpClientFactory(provider.GetRequiredService<IHttpClientFactory>())
                .WithLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
                .WithCredentialsConfiguration(credentialsBuilder =>
                {
                    var clientId = Environment.GetEnvironmentVariable("TEST_REDDIT_CLIENT_ID");
                    var clientSecret = Environment.GetEnvironmentVariable("TEST_REDDIT_CLIENT_SECRET");
                    var username = Environment.GetEnvironmentVariable("TEST_REDDIT_USERNAME");
                    var password = Environment.GetEnvironmentVariable("TEST_REDDIT_PASSWORD");

                    credentialsBuilder.Script(
                        clientId,
                        clientSecret,
                        username,
                        password);
                })
                .BuildAsync()
                .GetAwaiter()
                .GetResult();
        }
    }
}