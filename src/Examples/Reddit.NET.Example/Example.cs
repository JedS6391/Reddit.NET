using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            var client = await RedditClientBuilder
                .New
                .WithHttpClientFactory(_httpClientFactory)
                .WithLoggerFactory(_loggerFactory)                
                .WithCredentialsConfiguration(credentialsBuilder => 
                {
                    //ConfigureScriptCredentials(credentialsBuilder);
                    ConfigureWebAppCredentials(credentialsBuilder);
                })     
                .BuildAsync();

            var askReddit = client.Subreddit("askreddit");

            var askRedditDetails = await askReddit.GetDetailsAsync();            

            Console.WriteLine($"Subreddit [Name = {askRedditDetails.Name}, Title = {askRedditDetails.Title}]" );

            var topTenHotSubmissions = askReddit.GetHotSubmissionsAsync().Take(10);

            await foreach (var submission in topTenHotSubmissions)
            {
                Console.WriteLine($"Submission [Subreddit = {submission.Subreddit}, Title = {submission.Title}, Permalink = {submission.Permalink}]" );

                var interactor = submission.Interact(client);

                // await interactor.UpvoteAsync();

                await foreach (var comment in interactor.GetCommentsAsync())
                {
                    Console.WriteLine($"Comment [Body = {comment.Body}]");
                }
            }

            var me = client.Me(); 

            var meDetails = await me.GetDetailsAsync();

            Console.WriteLine($"User [Name = {meDetails.Name}]" );

            await foreach (var subreddit in me.GetSubredditsAsync())
            {            
                Console.WriteLine($"Subreddit [Name = {subreddit.Name}, Title = {subreddit.Title}]" );
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("StopAsync");

            return Task.CompletedTask;
        }

        private void ConfigureWebAppCredentials(CredentialsBuilder credentialsBuilder)
        {
            var clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");
            var redirectUri = new Uri(Environment.GetEnvironmentVariable("REDDIT_CLIENT_REDIRECT_URI"));
            var state = GetRandomState();
            
            var interactiveCredentialsBuilder = credentialsBuilder.WebApp(
                clientId,
                clientSecret,
                redirectUri,
                state);                

            Console.WriteLine("Please follow the steps to retrieve an access token and refresh token you can use with the Reddit.NET client.");
            Console.WriteLine();
             
            Console.WriteLine("1. Open the following link in your browser to complete the authorization process.");
            Console.WriteLine();
            Console.WriteLine($"{interactiveCredentialsBuilder.AuthorizationUri}");
            Console.WriteLine();

            Console.WriteLine("2. Once you've completed authorization in the browser, copy the final redirect URI and enter it below.");
            Console.WriteLine();

            var finalRedirectUri = PromptForValue("Final Redirect URI");

            var queryString = HttpUtility.ParseQueryString(new Uri(finalRedirectUri).Query);

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

            interactiveCredentialsBuilder.Authorize(codeParameter);
        }

        private void ConfigureScriptCredentials(CredentialsBuilder credentialsBuilder)
        {          
            var clientId = Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");
            var username = Environment.GetEnvironmentVariable("REDDIT_USERNAME");
            var password = Environment.GetEnvironmentVariable("REDDIT_PASSWORD");  

            var code = PromptForValue("2FA Code");

            credentialsBuilder.Script(
                clientId,
                clientSecret,
                username,
                $"{password}:{code}");
        }

        private static string PromptForValue(string valueName)
        {
            Console.Write($"Please enter your {valueName}: ");

            return Console.ReadLine();
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