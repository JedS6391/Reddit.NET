using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Provides the ability to navigate through a comment thread.
    /// </summary>
    /// <remarks>
    /// A <see cref="CommentThreadNavigator" /> manages a single level of comments on a submission.
    ///
    /// Each of the comments on the submission is itself represented as a <see cref="CommentThread" /> instance,
    /// of which the replies can be navigated through via a separate navigator instance.
    ///
    /// There is a special case of navigator for the top-level comments of a submission, where <see cref="Parent" />
    /// will be set to <see langword="null" />.
    /// </remarks>
    /// <example>
    /// Note that the replies in a thread can be directly enumerated over, as below:
    /// <code>
    /// CommentThreadNavigator navigator = ...;
    ///
    /// foreach (CommentThread topLevelThread in navigator)
    /// {
    ///     // Navigate replies of each thread
    ///     foreach (CommentThread replyThread in topLevelThread.Replies)
    ///     {
    ///         // Do something with reply thread.
    ///     }
    /// }
    /// </code>
    /// </example>
    public class CommentThreadNavigator : IReadOnlyList<CommentThread>
    {
        private readonly Submission _submission;
        private readonly Comment _parent;
        private readonly IReadOnlyList<Comment> _comments;
        private readonly IReadOnlyList<MoreComments> _moreComments;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentThreadNavigator" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor should be used for the root comment thread (i.e. when there is no parent comment).
        /// </remarks>
        /// <param name="submission">The submission the replies belong to.</param>
        /// <param name="replies">The replies to the submission.</param>
        internal CommentThreadNavigator(Submission submission, IReadOnlyList<IThing<IHasParent>> replies)
            : this(submission, replies, parent: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentThreadNavigator" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor should be used for nested (child) comment thread (i.e. when there is a parent comment).
        /// </remarks>
        /// <param name="submission">The submission the replies belong to.</param>
        /// <param name="replies">The replies to the submission.</param>
        /// <param name="parent">The parent comment the replies to belong to.</param>
        internal CommentThreadNavigator(
            Submission submission,
            IReadOnlyList<IThing<IHasParent>> replies,
            Comment parent)
        {
            _submission = submission;
            _parent = parent;
            _comments = replies.OfType<Comment>().ToList();
            // TODO: Need to provide a way to resolve more comment instances to the comments they represent,
            // and update the internal state with those new comments.
            // This is complicated for a number of reasons:
            //   1. Only 100 more comments can be resolved in a single call, so we need to batch these calls
            //   2. The comment IDs requested may not be what is actually returned. Reddit will return descendants
            //      of a requested comment, if that comment ranks higher than one of the other comments requested (based
            //      on the provided sort parameter)
            //   3. Because of (2), we can't just directly add comments as they may actually be descendants of other comments
            //      (i.e. not necessarily attached to the parent thread the current instance refers to)
            _moreComments = replies.OfType<MoreComments>().ToList();
        }

        /// <summary>
        /// Gets the parent comment thread if any.
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null" /> when the navigator is managing the top-level replies to a submission,
        /// otherwise will refer to the parent thread that the comments belong to.
        /// </remarks>
        public CommentThread Parent => _parent != null ?
            new CommentThread(_submission, _parent) :
            null;

        /// <inheritdoc />
        public CommentThread this[int index] => new CommentThread(_submission, _comments[index]);

        /// <inheritdoc />
        public int Count => _comments.Count;

        /// <inheritdoc />
        public IEnumerator<CommentThread> GetEnumerator() =>
            _comments.Select(c => new CommentThread(_submission, c)).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
