using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Command.Submissions;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Models.Public.Listings.Options;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the duplicates of a given submissions.
    /// </summary>
    public sealed class DuplicateSubmissionsListingEnumerable
        : ListingEnumerable<Submission.Listing, Submission.Details, SubmissionDetails, DuplicateSubmissionsListingEnumerable.Options>
    {
        private readonly RedditClient _client;
        private readonly ListingParameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateSubmissionsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        /// <param name="parameters">Parameters used when loading the listing data.</param>
        public DuplicateSubmissionsListingEnumerable(
            RedditClient client,
            Options options,
            ListingParameters parameters)
            : base(options)
        {
            _client = Requires.NotNull(client, nameof(client));
            _parameters = Requires.NotNull(parameters, nameof(parameters));
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
            var commandParameters = new GetDuplicateSubmissionsCommand.Parameters()
            {
                SubmissionId = _parameters.SubmissionId,
                Sort = ListingOptions.Sort.Name,
                Limit = ListingOptions.ItemsPerRequest,
                After = after
            };

            var getDuplicateSubmissionsCommand = new GetDuplicateSubmissionsCommand(commandParameters);

            // The Reddit API returns a list with two entries - a listing containing the original submission and a listing of the duplicates.
            var originalSubmissionAndDuplicates = await _client
                .ExecuteCommandAsync<IReadOnlyList<Submission.Listing>>(getDuplicateSubmissionsCommand, cancellationToken)
                .ConfigureAwait(false);

            return originalSubmissionAndDuplicates[1];
        }

        /// <summary>
        /// Defines parameters used when loading the listing data
        /// </summary>
        public class ListingParameters
        {
            /// <summary>
            /// Gets or sets the identifier of the submission to enumerate duplicates for.
            /// </summary>
            public string SubmissionId { get; set; }
        }

        /// <summary>
        /// Defines the options available for <see cref="DuplicateSubmissionsListingEnumerable" />.
        /// </summary>
        public class Options : ListingEnumerableOptions
        {
            /// <summary>
            /// Gets the option to use for sorting submissions.
            /// </summary>
            /// <remarks>Defaults to number of comments.</remarks>
            internal DuplicateSubmissionSort Sort { get; set; } = DuplicateSubmissionSort.NumberOfComments;

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
                /// <param name="sort">The option to use for sorting duplicate submissions.</param>
                /// <returns>The updated builder.</returns>
                public Builder WithSort(DuplicateSubmissionSort sort)
                {
                    Requires.NotNull(sort, nameof(sort));

                    Options.Sort = sort;

                    return this;
                }
            }
        }
    }
}
