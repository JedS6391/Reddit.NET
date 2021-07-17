using System.Linq;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Command.Subreddits;
using System.Threading;

namespace Reddit.NET.Client.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the submissions of a multireddit.
    /// </summary>
    public sealed class MultiredditSubmissionsListingEnumerable
        : ListingEnumerable<Submission.Listing, Submission.Details, SubmissionDetails, MultiredditSubmissionsListingEnumerable.Options>
    {
        private static readonly SubredditSubmissionSort[] s_sortOptionsSupportTimeRange = new SubredditSubmissionSort[]
        {
            SubredditSubmissionSort.Controversial,
            SubredditSubmissionSort.Top
        };

        private readonly RedditClient _client;
        private readonly ListingParameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiredditSubmissionsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        /// <param name="parameters">Parameters used when loading the listing data.</param>
        public MultiredditSubmissionsListingEnumerable(
            RedditClient client,
            Options options,
            ListingParameters parameters)
            : base(options)
        {
            _client = client;
            _parameters = parameters;
        }

        /// <inheritdoc />
        internal override async Task<Submission.Listing> GetInitialListingAsync(CancellationToken cancellationToken) =>
            await GetListingAsync(cancellationToken).ConfigureAwait(false);

        /// <inheritdoc />
        internal override async Task<Submission.Listing> GetNextListingAsync(Submission.Listing currentListing, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(cancellationToken, currentListing.Data.After).ConfigureAwait(false);
        }

        /// <inheritdoc />
        internal override SubmissionDetails MapThing(IThing<Submission.Details> thing) => new SubmissionDetails(thing);

        private async Task<Submission.Listing> GetListingAsync(CancellationToken cancellationToken, string after = null)
        {
            var commandParameters = new GetMultiredditSubmissionsCommand.Parameters()
            {
                Username = _parameters.Username,
                MultiredditName = _parameters.MultiredditName,
                Sort = ListingOptions.Sort.Name,
                Limit = ListingOptions.ItemsPerRequest,
                After = after
            };

            if (s_sortOptionsSupportTimeRange.Any(s => s.Name == ListingOptions.Sort.Name))
            {
                commandParameters.TimeRange = ListingOptions.TimeRange.Name;
            }

            var getMultiredditSubmissionsCommand = new GetMultiredditSubmissionsCommand(commandParameters);

            var submissions = await _client
                .ExecuteCommandAsync<Submission.Listing>(getMultiredditSubmissionsCommand, cancellationToken)
                .ConfigureAwait(false);

            return submissions;
        }

        /// <summary>
        /// Defines parameters used when loading the listing data
        /// </summary>
        public class ListingParameters
        {
            /// <summary>
            /// Gets or sets the name of the user the multireddit belongs to.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the name of the multireddit to load submissions from.
            /// </summary>
            public string MultiredditName { get; set; }
        }

        /// <summary>
        /// Defines the options available for <see cref="MultiredditSubmissionsListingEnumerable" />.
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
            /// Provides the ability to create <see cref="Options" /> instances.
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
