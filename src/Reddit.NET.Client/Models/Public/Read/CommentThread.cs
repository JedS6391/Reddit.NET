using Microsoft;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Public.Listings.Options;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a comment thread.
    /// </summary>
    /// <remarks>
    /// A comment thread is comprised of a top-level comment and all of its replies.
    /// </remarks>
    public class CommentThread
    {
        private readonly Submission _submission;
        private readonly Comment _comment;
        private readonly SubmissionsCommentSort _sort;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentThread" /> class.
        /// </summary>
        /// <param name="submission">The submission the thread belongs to.</param>
        /// <param name="comment">The top-level comment of the thread.</param>
        /// <param name="sort">The option the replies are sorted by.</param>
        internal CommentThread(Submission submission, Comment comment, SubmissionsCommentSort sort)
        {
            _submission = Requires.NotNull(submission, nameof(submission));
            _comment = Requires.NotNull(comment, nameof(comment));
            _sort = sort;
        }

        /// <summary>
        /// Gets the details of the submission the thread belongs to.
        /// </summary>
        public SubmissionDetails Submission => new SubmissionDetails(_submission);

        /// <summary>
        /// Gets the details of the top-level comment in the thread.
        /// </summary>
        public CommentDetails Details => new CommentDetails(_comment);

        /// <summary>
        /// Gets the replies to the top-level comment in the thread.
        /// </summary>
        /// <remarks>
        /// A <see cref="CommentThreadNavigator" /> instance is provided to allow further navigation through
        /// the replies which themselves are threads.
        /// </remarks>
        public CommentThreadNavigator Replies => new CommentThreadNavigator(
            _submission,
            _comment.Data.Replies.Children,
            _sort,
            parent: _comment);
    }
}
