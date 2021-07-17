using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Command.Users;
using System.Threading;

namespace Reddit.NET.Client.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the subreddits the authenticated user is subscribed to.
    /// </summary>
    public sealed class MySubredditsListingEnumerable
        : ListingEnumerable<Subreddit.Listing, Subreddit.Details, SubredditDetails, MySubredditsListingEnumerable.Options>
    {
        private readonly RedditClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySubredditsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        public MySubredditsListingEnumerable(RedditClient client, Options options)
            : base(options)
        {
            _client = client;
        }

        /// <inheritdoc />
        internal override async Task<Subreddit.Listing> GetInitialListingAsync(CancellationToken cancellationToken) =>
            await GetListingAsync(cancellationToken).ConfigureAwait(false);

        /// <inheritdoc />
        internal override async Task<Subreddit.Listing> GetNextListingAsync(Subreddit.Listing currentListing, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(cancellationToken, currentListing.Data.After).ConfigureAwait(false);
        }

        /// <inheritdoc />
        internal override SubredditDetails MapThing(IThing<Subreddit.Details> thing) => new SubredditDetails(thing);

        private async Task<Subreddit.Listing> GetListingAsync(CancellationToken cancellationToken, string after = null)
        {
            var getUserSubredditsCommand = new GetMySubredditsCommand(new GetMySubredditsCommand.Parameters()
            {
                After = after,
                Limit = ListingOptions.ItemsPerRequest
            });

            var subreddits = await _client
                .ExecuteCommandAsync<Subreddit.Listing>(getUserSubredditsCommand, cancellationToken)
                .ConfigureAwait(false);

            return subreddits;
        }

        /// <summary>
        /// Defines the options available for <see cref="MySubredditsListingEnumerable" />.
        /// </summary>
        public class Options : ListingEnumerableOptions
        {
            /// <summary>
            /// Provides the ability to create <see cref="Options" /> instances.
            /// </summary>
            public class Builder : ListingEnumerableOptionsBuilder<Options, Builder>
            {
                /// <inheritdoc />
                protected override Builder Instance => this;
            }
        }
    }
}
