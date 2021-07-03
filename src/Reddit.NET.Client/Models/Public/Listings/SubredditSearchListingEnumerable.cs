using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Command.Subreddits;

namespace Reddit.NET.Client.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over a search result of a subreddits submissions. 
    /// </summary>
    public sealed class SubredditSearchListingEnumerable
        : ListingEnumerable<Submission.Listing, Submission.Details, SubmissionDetails, SubredditSearchListingEnumerable.Options>
    {
        private readonly RedditClient _client;
        private readonly ListingParameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditSearchListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        /// <param name="parameters">Parameters used when loading the listing data.</param>
        public SubredditSearchListingEnumerable(
            RedditClient client,
            Options options,
            ListingParameters parameters)
            : base(options)
        {
            _client = client;
            _parameters = parameters;
        }
    
        /// <inheritdoc />
        internal override async Task<Submission.Listing> GetInitialListingAsync() => await GetListingAsync().ConfigureAwait(false);

        /// <inheritdoc />
        internal override async Task<Submission.Listing> GetNextListingAsync(Submission.Listing currentListing)
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
            var commandParameters = new SearchSubredditSubmissionsCommand.Parameters()
            {
                SubredditName = _parameters.SubredditName,
                Query = _parameters.Query,
                Syntax = ListingOptions.Syntax.Name,
                Sort = ListingOptions.Sort.Name,
                TimeRange = ListingOptions.TimeRange.Name,
                Limit = ListingOptions.ItemsPerRequest,
                After = after
            };

            var searchSubredditSubmissionsCommand = new SearchSubredditSubmissionsCommand(commandParameters);

            var submissions = await _client
                .ExecuteCommandAsync<Submission.Listing>(searchSubredditSubmissionsCommand)
                .ConfigureAwait(false);

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

            /// <summary>
            /// Gets or sets the query to search.
            /// </summary>
            public string Query { get; set; }
        }

        /// <summary>
        /// Defines the options available for <see cref="SubredditSubmissionsListingEnumerable" />.
        /// </summary>
        public class Options : ListingEnumerableOptions
        {
            /// <summary>
            /// Gets the option to use for sorting submissions.
            /// </summary>
            /// <remarks>Defaults to relevance.</remarks>
            internal SubredditSearchSort Sort { get; set; } = SubredditSearchSort.Relevance;

            /// <summary>
            /// Gets the option to use for the time range of submissions.
            /// </summary>
            /// <remarks>Defaults to all time.</remarks>
            internal TimeRangeSort TimeRange { get; set; } = TimeRangeSort.AllTime;

            /// <summary>
            /// Gets the search query syntax used.
            /// </summary>
            /// <remarks>Default to Lucene.</remarks>
            internal SearchQuerySyntax Syntax = SearchQuerySyntax.Lucene;            

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
                public Builder WithSort(SubredditSearchSort sort)
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

                /// <summary>
                /// Sets the search query syntax option.
                /// </summary>
                /// <param name="syntax">The option to use for the search query syntax.</param>
                /// <returns>The updated builder.</returns>
                public Builder WithSyntax(SearchQuerySyntax syntax)
                {
                    Options.Syntax = syntax;

                    return this;
                }     
            }
        }
    }
}