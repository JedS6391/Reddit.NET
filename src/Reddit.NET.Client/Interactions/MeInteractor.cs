using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Command.Users;
using Reddit.NET.Client.Interactions.Abstract;
using System.Linq;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with the authenticated user.
    /// </summary>
    public sealed class MeInteractor : IInteractor
    {
        private readonly RedditClient _client; 

        /// <summary>
        /// Initializes a new instance of the <see cref="MeInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        public MeInteractor(RedditClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets the details of the authenticated user.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the authenticated user.</returns>
        public async Task<UserDetails> GetDetailsAsync()
        {
            var getMyDetailsCommand = new GetMyDetailsCommand();

            // We use the data model type to deserialize the response as the user details
            // API returns a plain data object, rather than wrapping the data within a thing.
            var user = await _client.ExecuteCommandAsync<User.Details>(getMyDetailsCommand).ConfigureAwait(false);

            return new UserDetails(user);
        }

        /// <summary>
        /// Gets the subreddits the authenticated user is subscribed to.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the authenticated user's subreddits.</returns>
        public IAsyncEnumerable<SubredditDetails> GetSubredditsAsync(
            Action<MySubredditsListingEnumerable.Options.Builder> configurationAction = null) 
        {
            var optionsBuilder = new MySubredditsListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new MySubredditsListingEnumerable(_client, optionsBuilder.Options);
        }

        /// <summary>
        /// Gets the history of the authenticated user.
        /// </summary>
        /// <param name="configurationAction">An <see cref="Action{T}" /> used to configure listing options.</param>
        /// <returns>An asynchronous enumerator over the authenticated user's history.</returns>
        public IAsyncEnumerable<UserContentDetails> GetHistoryAsync(
            Action<UserHistoryListingEnumerable.Options.Builder> configurationAction = null)
        {
            var optionsBuilder = new UserHistoryListingEnumerable.Options.Builder();

            configurationAction?.Invoke(optionsBuilder);

            return new UserHistoryListingEnumerable(
                _client,
                optionsBuilder.Options,
                new UserHistoryListingEnumerable.ListingParameters()
                {
                    UseAuthenticatedUser = true
                });
        }

        /// <summary>
        /// Gets the karma breakdown of the authenticated user.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the karma breakdown of the authenticated user.</returns>
        public async Task<IReadOnlyList<KarmaBreakdownDetails>> GetKarmaBreakdownAsync()
        {
            var getMyKarmaBreakdownCommand = new GetMyKarmaBreakdownCommand();

            var karmaBreakdown = await _client
                .ExecuteCommandAsync<KarmaBreakdown>(getMyKarmaBreakdownCommand)
                .ConfigureAwait(false);
            
            return karmaBreakdown
                .Data
                .Select(kb => new KarmaBreakdownDetails(kb))
                .ToList();
        }      
    }
}