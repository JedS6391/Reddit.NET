using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.Extensions.Logging;
using Reddit.NET.Client.Builder;
using Reddit.NET.Client.Models.Public.Listings.Options;

namespace Reddit.NET.Console.Examples
{
    /// <summary>
    /// Demonstrates how to use a non-interactive authentication flow to configure the 
    /// client to interact with reddit on behalf of a user.
    /// </summary>
    internal class UsernamePasswordExample : IExample
    {
        private readonly ILogger<UsernamePasswordExample> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyExample" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> used to write messages.</param>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory" /> used to create logger instance.</param>
        /// <param name="httpClientFactory">A <see cref="IHttpClientFactory" /> used to create HTTP clients.</param>
        public UsernamePasswordExample(
            ILogger<UsernamePasswordExample> logger, 
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            _logger = Requires.NotNull(logger, nameof(logger));
            _loggerFactory = Requires.NotNull(loggerFactory, nameof(loggerFactory));
            _httpClientFactory = Requires.NotNull(httpClientFactory, nameof(httpClientFactory));
        }

        /// <inheritdoc />
        public string Name => "username-password";

        /// <inheritdoc />
        public async Task RunAsync()
        {        
            var client = await RedditClientBuilder
                .New
                .WithHttpClientFactory(_httpClientFactory)
                .WithLoggerFactory(_loggerFactory)                
                .WithCredentialsConfiguration(credentialsBuilder => 
                {
                    ConfigureScriptCredentials(credentialsBuilder);                    
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

            var overviewHistory = me.GetHistoryAsync(builder =>
                builder
                    .WithType(UserHistoryType.Overview)
                    .WithSort(UserHistorySort.Top)
                    .WithTimeRange(TimeRangeSort.AllTime)
                    .WithMaximumItems(50));

            await foreach (var item in overviewHistory)
            {
                _logger.LogInformation(item.ToString());
            }            
        }

        private void ConfigureScriptCredentials(CredentialsBuilder credentialsBuilder)
        {          
            var clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");
            var username = Environment.GetEnvironmentVariable("REDDIT_USERNAME");
            var password = Environment.GetEnvironmentVariable("REDDIT_PASSWORD");  

            // 2FA code is problematic for authentication with reddit as it may not be valid
            // when the authentication actually occurs.
            var code = PromptForValue("2FA Code");

            credentialsBuilder.Script(
                clientId,
                clientSecret,
                username,
                $"{password}:{code}");
        }

        private string PromptForValue(string valueName)
        {
            _logger.LogInformation($"Please enter your {valueName}: ");

            return System.Console.ReadLine();
        }
    }
}