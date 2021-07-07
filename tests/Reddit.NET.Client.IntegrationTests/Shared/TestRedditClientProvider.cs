using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reddit.NET.Client.Authentication.Credential;
using Reddit.NET.Client.Builder;

namespace Reddit.NET.Client.IntegrationTests.Shared
{
    /// <summary>
    /// Responsible for managing the <see cref="RedditClient" /> instances used by the integration tests.
    /// </summary>
    public static class TestRedditClientProvider
    {
        private static readonly Lazy<RedditClient> s_scriptClient = new Lazy<RedditClient>(() => BuildClient(AuthenticationMode.Script));
        private static readonly Lazy<RedditClient> s_readOnlyClient = new Lazy<RedditClient>(() => BuildClient(AuthenticationMode.ReadOnly));

        /// <summary>
        /// Gets a client instance configured with the script authentication mode.
        /// </summary>
        /// <returns>A <see cref="RedditClient" /> instance.</returns>
        public static RedditClient GetScriptClient() => s_scriptClient.Value;

        public static RedditClient GetReadOnlyClient() => s_readOnlyClient.Value;

        private static RedditClient BuildClient(AuthenticationMode mode)
        {
            var services = new ServiceCollection();

            services
                .AddLogging(builder =>
                    builder
                        .AddDebug()
                        .AddConsole()
                        .SetMinimumLevel(LogLevel.Error))
                .AddRedditHttpClient(userAgent: $"{Environment.OSVersion.Platform}:Reddit.NET.Client.IntegrationTests:v0.1.0 (by JedS6391)");

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

                    switch (mode)
                    {
                        case AuthenticationMode.Script:
                            credentialsBuilder.Script(clientId, clientSecret, username, password);

                            break;

                        case AuthenticationMode.ReadOnly:
                            credentialsBuilder.ReadOnly(clientId, clientSecret, deviceId: Guid.NewGuid());

                            break;

                        default:
                            throw new NotSupportedException($"Unsupported authentication mode '{mode}'");
                    }
                })
                .BuildAsync()
                .GetAwaiter()
                .GetResult();
        }
    }
}
