using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Command.Users;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the friends of the authenticated user.
    /// </summary>
    public sealed class MyFriendsListingEnumerable
        : ListingEnumerable<Friend.Listing, Friend.Details, FriendDetails, MyFriendsListingEnumerable.Options>
    {
        private readonly RedditClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyFriendsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        public MyFriendsListingEnumerable(RedditClient client, Options options)
            : base(options)
        {
            _client = Requires.NotNull(client, nameof(client));
        }

        /// <inheritdoc />
        internal override async Task<Friend.Listing> GetInitialListingAsync(CancellationToken cancellationToken) =>
            await GetListingAsync(cancellationToken).ConfigureAwait(false);

        /// <inheritdoc />
        internal override async Task<Friend.Listing> GetNextListingAsync(Friend.Listing currentListing, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(cancellationToken, currentListing.Data.After).ConfigureAwait(false);
        }

        /// <inheritdoc />
        internal override FriendDetails MapThing(IThing<Friend.Details> thing) => new FriendDetails(thing);

        private async Task<Friend.Listing> GetListingAsync(CancellationToken cancellationToken, string after = null)
        {
            var getMyFriendsCommand = new GetMyFriendsCommand(new GetMyFriendsCommand.Parameters()
            {
                After = after,
                Limit = ListingOptions.ItemsPerRequest
            });

            var friends = await _client
                .ExecuteCommandAsync<Friend.Listing>(getMyFriendsCommand, cancellationToken)
                .ConfigureAwait(false);

            return friends;
        }

        /// <summary>
        /// Defines the options available for <see cref="MyFriendsListingEnumerable" />.
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
