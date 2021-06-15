using System;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Users;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the subreddits a user is subscribed to. 
    /// </summary>
    public sealed class UserSubredditsListingEnumerable
        : ListingEnumerable<Subreddit.Listing, Subreddit.Details, SubredditDetails, UserSubredditsListingEnumerable.Options>
    {
        private readonly RedditClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSubredditsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        public UserSubredditsListingEnumerable(
            RedditClient client, 
            UserSubredditsListingEnumerable.Options options)
            : base(options)
        {
            _client = client;
        }

        /// <inheritdoc />
        internal async override Task<Subreddit.Listing> GetInitialListingAsync() => await GetListingAsync().ConfigureAwait(false);

        /// <inheritdoc />
        internal async override Task<Subreddit.Listing> GetNextListingAsync(Subreddit.Listing currentListing)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(currentListing.Data.After).ConfigureAwait(false);
        }
        
        /// <inheritdoc />
        internal override SubredditDetails MapThing(Thing<Subreddit.Details> thing) => new SubredditDetails(thing);

        private async Task<Subreddit.Listing> GetListingAsync(string after = null)
        {
            var getUserSubredditsCommand = new GetUserSubredditsCommand(new GetUserSubredditsCommand.Parameters()
            {
                After = after,
                Limit = ListingOptions.ItemsPerRequest
            });

            var subreddits = await _client.ExecuteCommandAsync<Subreddit.Listing>(getUserSubredditsCommand);

            return subreddits;
        } 

        /// <summary>
        /// Defines the options available for <see cref="UserSubredditsListingEnumerable" />.
        /// </summary>
        public class Options : ListingEnumerableOptions
        {
            /// <summary>
            /// Provides the ability to create <see cref="UserSubredditsListingEnumerable.Options" /> instances.
            /// </summary>
            public class Builder : ListingEnumerableOptionsBuilder<Options, Builder>
            {
                /// <inheritdoc />
                protected override Builder Instance => this;
            }
        }
    }
}