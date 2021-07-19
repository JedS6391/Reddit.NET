using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Command.Submissions;
using Reddit.NET.Client.Interactions.Abstract;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a comment.
    /// </summary>
    public sealed class CommentInteractor : UserContentInteractor, IInteractor
    {
        private readonly string _submissionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentInteractor" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="submissionId">The base-36 ID of the submission the comment belongs to.</param>
        /// <param name="commentId">The base-36 ID of the comment to interact with.</param>
        internal CommentInteractor(RedditClient client, string submissionId, string commentId)
            : base(client, kind: Constants.Kind.Comment, id: commentId)
        {
            _submissionId = Requires.NotNull(submissionId, nameof(submissionId));
        }

        /// <summary>
        /// Gets the details of the comment.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the comment.</returns>
        public async Task<CommentDetails> GetDetailsAsync(CancellationToken cancellationToken = default)
        {
            var commandParameters = new GetSubmissionDetailsWithCommentsCommand.Parameters()
            {
                SubmissionId = _submissionId,
                // Only load the data (including replies) for this comment.
                FocusCommentId = Id
            };

            var getSubmissionDetailsWithCommentsCommand = new GetSubmissionDetailsWithCommentsCommand(commandParameters);

            var submissionWithComments = await Client
                .ExecuteCommandAsync<Submission.SubmissionWithComments>(getSubmissionDetailsWithCommentsCommand, cancellationToken)
                .ConfigureAwait(false);

            // Find the comment in the listing returned. The listing should have only one child which is
            // the comment we're interested in, but this approach should be safer than just directly accessing
            // the first entry in the listing children collection.
            var comment = submissionWithComments
                .Comments
                .Children
                .OfType<Comment>()
                .First(c => c.Data.Id == Id);

            return new CommentDetails(comment);
        }
    }
}
