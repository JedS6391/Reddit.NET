using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Public.Listings;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Subreddits;
using Reddit.NET.Core.Client.Interactions.Abstract;

namespace Reddit.NET.Core.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a subreddit.
    /// </summary>
    public class SubredditInteractor : IInteractor
    {
        private readonly RedditClient _client;
        private readonly string _subredditName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>        
        /// <param name="subredditName">The name of the subreddit to interact with.</param>
        public SubredditInteractor(
            RedditClient client,
            string subredditName)
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

            var subreddit = await _client.ExecuteCommandAsync<Subreddit>(getSubredditDetailsCommand);

            return new SubredditDetails(subreddit);
        }

    
        /// <summary>
        /// Gets the submissions of the subreddit.
        /// </summary>
        /// <returns>An asynchronous enumerator over the submissions of the subreddit.</returns>
        public IAsyncEnumerable<SubmissionDetails> GetSubmissionsAsync(
            Action<SubredditSubmissionsListingEnumerable.Options.Builder> optionsBuilderAction = null)
        {
            var optionsBuilder = new SubredditSubmissionsListingEnumerable.Options.Builder();            
    
            optionsBuilderAction?.Invoke(optionsBuilder);

            return new SubredditSubmissionsListingEnumerable(
                _client,
                optionsBuilder,
                new SubredditSubmissionsListingEnumerable.ListingParameters()
                {
                    SubredditName = _subredditName
                });
        }
    }
}