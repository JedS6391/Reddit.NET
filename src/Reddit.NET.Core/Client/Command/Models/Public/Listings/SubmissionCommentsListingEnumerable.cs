using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Submissions;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped}" /> implementation over the comments of a submission. 
    /// </summary>
    public class SubmissionCommentsListingEnumerable
        : ListingEnumerable<Comment.Listing, Comment.Details, CommentDetails>
    {
        private readonly RedditClient _client;
        private readonly SubmissionCommentsListingEnumerable.ListingParameters _parameters;        

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSubredditsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// /// <param name="parameters">Parameters used when loading the listing data.</param>
        public SubmissionCommentsListingEnumerable(
            RedditClient client,
            SubmissionCommentsListingEnumerable.ListingParameters parameters)
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
        internal override CommentDetails MapThing(Thing<Comment.Details> thing) => new CommentDetails(thing);

        private async Task<Comment.Listing> GetListingAsync(string after = null)
        {
            var getSubmissionCommentsCommand = new GetSubmissionCommentsCommand(new GetSubmissionCommentsCommand.Parameters()
            {
                SubredditName = _parameters.SubredditName,
                SubmissionId = _parameters.SubmissionId
            });
        
            var submissionComments = await _client.ExecuteCommandAsync<Submission.SubmissionComments>(getSubmissionCommentsCommand);            

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
    }
}