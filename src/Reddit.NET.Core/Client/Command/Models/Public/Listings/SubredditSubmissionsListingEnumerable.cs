using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Subreddits;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the submissions of a subreddit.. 
    /// </summary>
    public class SubredditSubmissionsListingEnumerable
        : ListingEnumerable<Submission.Listing, Submission.Details, SubmissionDetails, SubredditSubmissionsListingEnumerable.Options>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditSubmissionsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        /// <param name="parameters">Parameters used when loading the listing data.</param>
        public SubredditSubmissionsListingEnumerable(
            RedditClient client,
            SubredditSubmissionsListingEnumerable.Options options,
            SubredditSubmissionsListingEnumerable.ListingParameters parameters)
            : base(options)
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

        private async Task<Submission.Listing> GetListingAsync(string after = null)
        {
            var getSubredditSubmissionsCommand = new GetSubredditSubmissionsCommand(new GetSubredditSubmissionsCommand.Parameters()
            {
                SubredditName = Parameters.SubredditName,
                Sort = ListingOptions.Sort.Name,
                Limit = ListingOptions.ItemsPerRequest,
                After = after
            });

            var submissions = await Client.ExecuteCommandAsync<Submission.Listing>(getSubredditSubmissionsCommand);

            return submissions;    
        }

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

        /// <summary>
        /// Defines the options available for <see cref="SubredditSubmissionsListingEnumerable" />.
        /// </summary>
        public class Options : ListingEnumerableOptions
        {
            /// <summary>
            /// Gets the order to use for sorting submissions.
            /// </summary>
            /// <remarks>Defaults to hot.</remarks>
            internal SubredditSubmissionSort Sort { get; set; } = SubredditSubmissionSort.Hot;

            /// <summary>
            /// Provides the ability to create <see cref="UserSubredditsListingEnumerable.Options" /> instances.
            /// </summary>
            public class Builder : ListingEnumerableOptionsBuilder<Options, Builder>
            {
                /// <inheritdoc />
                protected override Builder Instance => this;

                /// <summary>
                /// Sets the number of items to retrieve per request.
                /// </summary>
                /// <param name="sort">The order to use for sorting submissions.</param>
                /// <returns>The updated builder.</returns>
                public Builder WithSort(SubredditSubmissionSort sort)
                {
                    Options.Sort = sort;

                    return this;
                }          
            }
        }
    }
}