using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using Microsoft;
using Microsoft.Extensions.Logging;
using Reddit.NET.Client.Builder;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Listings;

namespace Reddit.NET.Console.Examples
{
    /// <summary>
    /// Demonstrates how to use an interactive authentication flow to configure the
    /// client to interact with reddit on behalf of a user.
    /// </summary>
    internal class AuthorizationCodeExample : IExample
    {
        private readonly ILogger<AuthorizationCodeExample> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationCodeExample" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> used to write messages.</param>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory" /> used to create logger instance.</param>
        /// <param name="httpClientFactory">A <see cref="IHttpClientFactory" /> used to create HTTP clients.</param>
        public AuthorizationCodeExample(
            ILogger<AuthorizationCodeExample> logger,
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            _logger = Requires.NotNull(logger, nameof(logger));
            _loggerFactory = Requires.NotNull(loggerFactory, nameof(loggerFactory));
            _httpClientFactory = Requires.NotNull(httpClientFactory, nameof(httpClientFactory));
        }

        /// <inheritdoc />
        public string Name => "authorization-code";

        /// <inheritdoc />
        public async Task RunAsync()
        {
            var client = await RedditClientBuilder
                .New
                .WithHttpClientFactory(_httpClientFactory)
                .WithLoggerFactory(_loggerFactory)
                .WithCredentialsConfiguration(credentialsBuilder =>
                {
                    ConfigureWebAppCredentials(credentialsBuilder);
                })
                .BuildAsync();

            var askReddit = client.Subreddit("askreddit");

            var askRedditDetails = await askReddit.GetDetailsAsync();

            _logger.LogInformation(askRedditDetails.ToString());

            var topFiftyHotSubmissions = askReddit.GetSubmissionsAsync(builder =>
                builder
                    .WithSort(SubredditSubmissionSort.Hot)
                    .WithMaximumItems(50));

            await foreach (var submission in topFiftyHotSubmissions)
            {
                _logger.LogInformation(submission.ToString());
            }

            var me = client.Me();

            var meDetails = await me.GetDetailsAsync();

            _logger.LogInformation(meDetails.ToString());

            await foreach (var subreddit in me.GetSubredditsAsync())
            {
                _logger.LogInformation(subreddit.ToString());
            }

            var savedHistory = me.GetHistoryAsync(builder =>
                builder
                    .WithType(UserHistoryType.Saved)
                    .WithMaximumItems(50));

            await foreach (var item in savedHistory)
            {
                _logger.LogInformation(item.ToString());
            }
        }

        private void ConfigureWebAppCredentials(CredentialsBuilder credentialsBuilder)
        {
            var clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");
            var redirectUri = Environment.GetEnvironmentVariable("REDDIT_CLIENT_REDIRECT_URI");
            var state = GetRandomState();

            // With an interactive authentication mode, we need to further configure via
            // the interactive credentials builder.
            // The interactive credentials builder provides access to the authorization URI
            // the user should be sent to and requires the authorization code to be set once
            // the authorization flow has completed.
            var interactiveCredentialsBuilder = credentialsBuilder.WebApp(
                clientId,
                clientSecret,
                new Uri(redirectUri),
                state);

            var authorizationUri = interactiveCredentialsBuilder.GetAuthorizationUri();

            // Send the user to the authorization URI.
            _logger.LogInformation("Please follow the steps to retrieve an access token and refresh token you can use with the Reddit.NET client.\n");
            _logger.LogInformation("1. Open the following link in your browser to complete the authorization process:\n");
            _logger.LogInformation($"{authorizationUri}\n");
            _logger.LogInformation("2. Once you've completed authorization in the browser, copy the final redirect URI and enter it below.\n");

            var finalRedirectUriString = PromptForValue("Final Redirect URI");
            var finalRedirectUri = new Uri(finalRedirectUriString);

            var queryString = HttpUtility.ParseQueryString(finalRedirectUri.Query);

            // Validate state and extract the code.
            var stateParameter = queryString.Get("state");
            var codeParameter = queryString.Get("code");

            if (string.IsNullOrEmpty(stateParameter) || stateParameter != state)
            {
                throw new InvalidOperationException("State parameter not found or does not match.");
            }

            if (string.IsNullOrEmpty(codeParameter))
            {
                throw new InvalidOperationException("Code parameter not found.");
            }

            // Complete the flow.
            interactiveCredentialsBuilder.Authorize(codeParameter);
        }

        private string PromptForValue(string valueName)
        {
            _logger.LogInformation($"Please enter your {valueName}: ");

            return System.Console.ReadLine();
        }

        private static string GetRandomState()
        {
            using var random = new RNGCryptoServiceProvider();

            var tokenData = new byte[32];

            random.GetBytes(tokenData);

            return Convert.ToBase64String(tokenData);
        }
    }
}
