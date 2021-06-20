using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Models.Public.ReadOnly;
using Reddit.NET.Client.Command.Submissions;

namespace Reddit.NET.Client.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the comments of a submission. 
    /// </summary>
    public class SubmissionCommentsListingEnumerable
        : ListingEnumerable<Comment.Listing, Comment.Details, CommentDetails, SubmissionCommentsListingEnumerable.Options>
    {
        private readonly RedditClient _client;
        private readonly SubmissionCommentsListingEnumerable.ListingParameters _parameters;        

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionCommentsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        /// <param name="parameters">Parameters used when loading the listing data.</param>
        public SubmissionCommentsListingEnumerable(
            RedditClient client,
            SubmissionCommentsListingEnumerable.Options options,
            SubmissionCommentsListingEnumerable.ListingParameters parameters)
            : base(options)
        {
            _client = client;
            _parameters = parameters;
        }

        /// <inheritdoc />
        internal async override Task<Comment.Listing> GetInitialListingAsync() => await GetListingAsync().ConfigureAwait(false);

        /// <inheritdoc />
        internal async override Task<Comment.Listing> GetNextListingAsync(Comment.Listing currentListing)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(currentListing.Data.After).ConfigureAwait(false);
        }

        /// <inheritdoc />
        internal override CommentDetails MapThing(IThing<Comment.Details> thing) => new CommentDetails(thing);

        private async Task<Comment.Listing> GetListingAsync(string after = null)
        {
            // TODO: This doesn't handle the after parameter. Is it applicable for comments?
            var getSubmissionCommentsCommand = new GetSubmissionCommentsCommand(new GetSubmissionCommentsCommand.Parameters()
            {
                SubredditName = _parameters.SubredditName,
                SubmissionId = _parameters.SubmissionId
            });

            var submissionComments = await _client
                .ExecuteCommandAsync<Submission.SubmissionComments>(getSubmissionCommentsCommand)
                .ConfigureAwait(false);

            // TODO: Would be nice to have the link between submission and comment.
            return submissionComments.Comments;
        }    

        /// <summary>
        /// Defines parameters used when loading the listing data
        /// </summary>
        public class ListingParameters 
        {
            /// <summary>
            /// Gets or sets the name of the subreddit the submission belongs to
            /// </summary>
            public string SubredditName { get; set; }

            /// <summary>
            /// Gets or sets the identifier of the submission to load comments for.
            /// </summary>
            public string SubmissionId { get; set; }
        }

        /// <summary>
        /// Defines the options available for <see cref="SubmissionCommentsListingEnumerable" />.
        /// </summary>
        public class Options : ListingEnumerableOptions
        {
            /// <summary>
            /// Provides the ability to create <see cref="SubmissionCommentsListingEnumerable.Options" /> instances.
            /// </summary>
            public class Builder : ListingEnumerableOptionsBuilder<Options, Builder>
            {
                /// <inheritdoc />
                protected override Builder Instance => this;
            }
        }
    }
}