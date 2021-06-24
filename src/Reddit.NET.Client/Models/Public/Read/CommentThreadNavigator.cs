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
    /// <example>
    /// Note that the replies in a thread can be directly enumerated over, as below:
    /// <code>
    /// CommentThreadNavigator navigator = ...;
    /// 
    /// foreach (CommentThread thread in navigator)
    /// {
    ///     // Do something with thread
    ///     ...
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
            _comments = replies
                .Where(c => c is Comment)
                .Select(c => c as Comment)
                .ToList();
            _moreComments = replies
                .Where(c => c is MoreComments)
                .Select(c => c as MoreComments)
                .ToList();
        }

        /// <summary>
        /// Gets the parent comment thread if any.
        /// </summary>
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
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}