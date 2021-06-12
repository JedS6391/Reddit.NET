using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client
{
    /// <summary>
    /// Provides mechanisms for interacting with <see href="https://www.reddit.com">reddit</see>. 
    /// </summary>
    /// <remarks>
    /// <see cref="RedditClient" /> cannot be directly instantiated and should instead be created via the <see cref="Builder.RedditClientBuilder" /> class.
    /// </remarks>
    public sealed partial class RedditClient
    {
        private readonly ILogger<RedditClient> _logger;
        private readonly CommandExecutor _commandExecutor;
        private readonly IAuthenticator _authenticator;        

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClient" /> class.
        /// </summary>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory" /> instance used create a logger for writing log messages.</param>
        /// <param name="httpClientFactory">An <see cref="IHttpClientFactory" /> instance used to create clients when executing requests</param>
        /// <param name="authenticator">An <see cref="IAuthenticator" /> instance used to authenticate with reddit.</param>
        internal RedditClient(
            ILogger<RedditClient> logger,
            CommandExecutor commandExecutor,
            IAuthenticator authenticator)
        {
            _logger = logger;
            _commandExecutor = commandExecutor;
            _authenticator = authenticator;
        }

        /// <summary>
        /// Gets an interactor for operations relating to the authenticated user.
        /// </summary>
        /// <returns>A <see cref="MeInteractor" /> instance that provides mechanisms for interacting with the authenticated user.</returns>
        public MeInteractor Me() => new MeInteractor(this);
        
        /// <summary>
        /// Gets an interactor for operations relating to a specific subreddit.
        /// </summary>
        /// <param name="name">The name of the subreddit to interact with.</param>
        /// <returns>A <see cref="SubredditInteractor" /> instance that provides mechanisms for interacting with the requested subreddit.</returns>
        public SubredditInteractor Subreddit(string name) => new SubredditInteractor(this, name);        
    }
}