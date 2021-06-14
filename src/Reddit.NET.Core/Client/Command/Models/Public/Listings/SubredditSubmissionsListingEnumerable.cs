using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped}" /> implementation over the submissions of a subreddit.. 
    /// </summary>
    public abstract class SubredditSubmissionsListingEnumerable
        : ListingEnumerable<Submission.Listing, Submission.Details, SubmissionDetails>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditSubmissionsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="parameters">Parameters used when loading the listing data.</param>
        protected SubredditSubmissionsListingEnumerable(
            RedditClient client,
            SubredditSubmissionsListingEnumerable.ListingParameters parameters)
        {
            Client = client;
            Parameters = parameters;
        }

        /// <summary>
        /// Gets a <see cref="RedditClient" /> instance used to load the listing data.
        /// </summary>
        protected RedditClient Client { get; }

        /// <summary>
        /// Gets the parameters used when loading the listing data.
        /// </summary>
        protected SubredditSubmissionsListingEnumerable.ListingParameters Parameters { get; }

        /// <inheritdoc />
        internal abstract Task<Submission.Listing> GetListingAsync(string after = null);

        /// <inheritdoc />
        internal async override Task<Submission.Listing> GetInitialListingAsync() => await GetListingAsync().ConfigureAwait(false);

        /// <inheritdoc />
        internal async override Task<Submission.Listing> GetNextListingAsync(Submission.Listing currentListing)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(currentListing.Data.After).ConfigureAwait(false);
        }

        /// <inheritdoc />
        internal override SubmissionDetails MapThing(Thing<Submission.Details> thing) => new SubmissionDetails(thing);

        /// <summary>
        /// Defines parameters used when loading the listing data
        /// </summary>
        public class ListingParameters 
        {
            /// <summary>
            /// Gets or sets the name of the subreddit to load submissions from.
            /// </summary>
            public string SubredditName { get; set; }
        }
    }
}