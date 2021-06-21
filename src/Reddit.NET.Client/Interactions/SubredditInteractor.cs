using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.ReadOnly;
using Reddit.NET.Client.Command.Subreddits;
using Reddit.NET.Client.Interactions.Abstract;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a subreddit.
    /// </summary>
    public sealed class SubredditInteractor : IInteractor
    {
        private readonly RedditClient _client;
        private readonly string _subredditName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>        
        /// <param name="subredditName">The name of the subreddit to interact with.</param>
        public SubredditInteractor(RedditClient client, string subredditName)
        {
            _client = client;
            _subredditName = subredditName;
        }

        /// <summary>
        /// Gets the details of the subreddit.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the subreddit.</returns>
        public async Task<SubredditDetails> GetDetailsAsync()
        {
            var getSubredditDetailsCommand = new GetSubredditDetailsCommand(new GetSubredditDetailsCommand.Parameters()
            {
                SubredditName = _subredditName
            });

            var subreddit = await _client.ExecuteCommandAsync<Subreddit>(getSubredditDetailsCommand).ConfigureAwait(false);

            return new SubredditDetails(subreddit);
        }

        /// <summary>
        /// Subscribes to the subreddit.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SubscribeAsync() =>
            await UpdateSubredditSubscriptionAsync(UpdateSubredditSubscriptionCommand.SubscriptionAction.Subscribe);

        /// <summary>
        /// Unsubscribes from the subreddit.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UnsubscribeAsync() =>
            await UpdateSubredditSubscriptionAsync(UpdateSubredditSubscriptionCommand.SubscriptionAction.Unubscribe);

        /// <summary>
        /// Gets the submissions of the subreddit.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the submissions of the subreddit.</returns>
        public IAsyncEnumerable<SubmissionDetails> GetSubmissionsAsync(
            Action<SubredditSubmissionsListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new SubredditSubmissionsListingEnumerable.Options.Builder();            
    
            configurationAction?.Invoke(optionsBuilder);

            return new SubredditSubmissionsListingEnumerable(
                _client,
                optionsBuilder.Options,
                new SubredditSubmissionsListingEnumerable.ListingParameters()
                {
                    SubredditName = _subredditName
                });
        }

        private async Task UpdateSubredditSubscriptionAsync(UpdateSubredditSubscriptionCommand.SubscriptionAction action)
        {
            var commandParameters = new UpdateSubredditSubscriptionCommand.Parameters()
            {
                SubredditName = _subredditName,
                Action = action
            };

            var updateSubredditSubscriptionCommand = new UpdateSubredditSubscriptionCommand(commandParameters);

            await _client.ExecuteCommandAsync(updateSubredditSubscriptionCommand).ConfigureAwait(false);             
        }
    }
}