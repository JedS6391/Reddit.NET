using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Builder;

namespace Reddit.NET.Console.Examples
{
    /// <summary>
    /// Demonstrates how to use a non-interactive authentication flow to configure the 
    /// client to interact with reddit with no user.
    /// </summary>
    internal class ReadOnlyExample : IExample
    {
        private readonly ILogger<ReadOnlyExample> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHttpClientFactory _httpClientFactory;        

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyExample" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}" /> used to write messages.</param>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory" /> used to create logger instance.</param>
        /// <param name="httpClientFactory">A <see cref="IHttpClientFactory" /> used to create HTTP clients.</param>        
        public ReadOnlyExample(
            ILogger<ReadOnlyExample> logger, 
            ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            _logger = Requires.NotNull(logger, nameof(logger));
            _loggerFactory = Requires.NotNull(loggerFactory, nameof(loggerFactory));
            _httpClientFactory = Requires.NotNull(httpClientFactory, nameof(httpClientFactory));
        }

        /// <inheritdoc />
        public string Name => "read-only";

        /// <inheritdoc />
        public async Task RunAsync()
        {        
            var client = await RedditClientBuilder
                .New
                .WithHttpClientFactory(_httpClientFactory)
                .WithLoggerFactory(_loggerFactory)                
                .WithCredentialsConfiguration(credentialsBuilder => 
                {
                    ConfigureReadOnlyCredentials(credentialsBuilder);                    
                })     
                .BuildAsync();

            var askReddit = client.Subreddit("askreddit");

            var askRedditDetails = await askReddit.GetDetailsAsync();            

            _logger.LogInformation(askRedditDetails.ToString());

            var topTenHotSubmissions = askReddit.GetHotSubmissionsAsync().Take(10);

            await foreach (var submission in topTenHotSubmissions)
            {
                _logger.LogInformation(submission.ToString());
            }

            
            var me = client.Me(); 

            // This will fail as read-only mode does not have access to use details
            try 
            {
                var meDetails = await me.GetDetailsAsync();
                
                _logger.LogInformation(meDetails.ToString());              
            }
            catch (Exception)
            {
                _logger.LogWarning("Cannot interact with user when using read-only authentication mode.");
            }
        }

        private static void ConfigureReadOnlyCredentials(CredentialsBuilder credentialsBuilder)
        {          
            var clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");
    
            credentialsBuilder.ReadOnly(
                clientId,
                clientSecret,
                deviceId: Guid.NewGuid());
        }
    }
}