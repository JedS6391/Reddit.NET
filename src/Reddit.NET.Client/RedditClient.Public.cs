using System;
using System.Collections.Generic;
using Microsoft;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Interactions;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client
{
    /// <summary>
    /// Provides mechanisms for interacting with <see href="https://www.reddit.com">reddit</see>.
    /// </summary>
    /// <remarks>
    /// <see cref="RedditClient" /> cannot be directly instantiated and should instead be created via the <see cref="Builder.RedditClientBuilder" /> class.
    /// </remarks>
    public sealed partial class RedditClient
    {
        private readonly CommandExecutor _commandExecutor;
        private readonly IAuthenticator _authenticator;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClient" /> class.
        /// </summary>
        /// <param name="commandExecutor">An <see cref="CommandExecutor" /> instance used to execute commands against reddit.</param>
        /// <param name="authenticator">An <see cref="IAuthenticator" /> instance used to authenticate with reddit.</param>
        internal RedditClient(CommandExecutor commandExecutor, IAuthenticator authenticator)
        {
            _commandExecutor = Requires.NotNull(commandExecutor, nameof(commandExecutor));
            _authenticator = Requires.NotNull(authenticator, nameof(authenticator));
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

        /// <summary>
        /// Gets an interactor for operations relating to a group of subreddits.
        /// </summary>
        /// <remarks>
        /// Note that some operations will not be possible when combining subreddits, e.g. you cannot create a new submission.
        /// </remarks>
        /// <param name="names">The names of the subreddits to interact with.</param>
        /// <returns>A <see cref="SubredditInteractor" /> instance that provides mechanisms for interacting with the requested subreddits.</returns>
        public SubredditInteractor Subreddits(params string[] names) => new SubredditInteractor(this, string.Join('+', names));

        /// <summary>
        /// Gets an interactor for operations relating to a specific user.
        /// </summary>
        /// <param name="name">The name of the user to interact with.</param>
        /// <returns>An <see cref="UserInteractor" /> instance that provides mechanisms for interacting with the requested user.</returns>
        public UserInteractor User(string name) => new UserInteractor(this, name);

        /// <summary>
        /// Gets the submissions on the front page.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the submissions on the front page.</returns>
        public IAsyncEnumerable<SubmissionDetails> GetFrontPageSubmissionsAsync(
            Action<FrontPageSubmissionsListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new FrontPageSubmissionsListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new FrontPageSubmissionsListingEnumerable(
                this,
                optionsBuilder.Options);
        }
    }
}
