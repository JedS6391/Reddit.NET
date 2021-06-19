using System.Linq;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Command.Models.Public.Listings.Sorting;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Subreddits;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the submissions of a subreddit.. 
    /// </summary>
    public sealed class SubredditSubmissionsListingEnumerable
        : ListingEnumerable<Submission.Listing, Submission.Details, SubmissionDetails, SubredditSubmissionsListingEnumerable.Options>
    {
        private static readonly SubredditSubmissionSort[] s_sortOptionsSupportTimeRange = new SubredditSubmissionSort[]
        {
            SubredditSubmissionSort.Controversial,
            SubredditSubmissionSort.Top
        };

        private readonly RedditClient _client;
        private readonly SubredditSubmissionsListingEnumerable.ListingParameters _parameters;

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
            _client = client;
            _parameters = parameters;
        }
    
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
        internal override SubmissionDetails MapThing(IThing<Submission.Details> thing) => new SubmissionDetails(thing);

        private async Task<Submission.Listing> GetListingAsync(string after = null)
        {
            var commandParameters = new GetSubredditSubmissionsCommand.Parameters()
            {
                SubredditName = _parameters.SubredditName,
                Sort = ListingOptions.Sort.Name,
                Limit = ListingOptions.ItemsPerRequest,
                After = after
            };

            if (s_sortOptionsSupportTimeRange.Any(s => s.Name == ListingOptions.Sort.Name))
            {
                commandParameters.TimeRange = ListingOptions.TimeRange.Name;
            }

            var getSubredditSubmissionsCommand = new GetSubredditSubmissionsCommand(commandParameters);

            var submissions = await _client.ExecuteCommandAsync<Submission.Listing>(getSubredditSubmissionsCommand);

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
            /// Gets the option to use for sorting submissions.
            /// </summary>
            /// <remarks>Defaults to hot.</remarks>
            internal SubredditSubmissionSort Sort { get; set; } = SubredditSubmissionSort.Hot;

            /// <summary>
            /// Gets the option to use for the time range of submissions.
            /// </summary>
            /// <remarks>Defaults to day.</remarks>
            internal TimeRangeSort TimeRange { get; set; } = TimeRangeSort.Day;

            /// <summary>
            /// Provides the ability to create <see cref="SubredditSubmissionsListingEnumerable.Options" /> instances.
            /// </summary>
            public class Builder : ListingEnumerableOptionsBuilder<Options, Builder>
            {
                /// <inheritdoc />
                protected override Builder Instance => this;

                /// <summary>
                /// Sets the sort option.
                /// </summary>
                /// <param name="sort">The option to use for sorting submissions.</param>
                /// <returns>The updated builder.</returns>
                public Builder WithSort(SubredditSubmissionSort sort)
                {
                    Options.Sort = sort;

                    return this;
                } 

                /// <summary>
                /// Sets the time range option.
                /// </summary>
                /// <param name="timeRange">The option to use for the time range of submissions.</param>
                /// <returns>The updated builder.</returns>
                public Builder WithTimeRange(TimeRangeSort timeRange)
                {
                    Options.TimeRange = timeRange;

                    return this;
                }      
            }
        }
    }
}