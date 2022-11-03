using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Command.Submissions;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Listings.Options;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Provides the ability to navigate through a comment thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="CommentThreadNavigator" /> manages a single level of comments on a submission.
    /// </para>
    /// <para>
    /// Each of the comments on the submission is itself represented as a <see cref="CommentThread" /> instance,
    /// of which the replies can be navigated through via a separate navigator instance.
    /// </para>
    /// <para>
    /// There is a special case of navigator for the top-level comments of a submission, where <see cref="Parent" />
    /// will be set to <see langword="null" />.
    /// </para>
    /// <para>
    /// </para>
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
        private readonly SubmissionsCommentSort _sort;
        private readonly List<Comment> _comments;
        private List<MoreComments> _moreComments;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentThreadNavigator" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor should be used for the root comment thread (i.e. when there is no parent comment).
        /// </remarks>
        /// <param name="submission">The submission the replies belong to.</param>
        /// <param name="sort">The option the replies are sorted by.</param>
        /// <param name="replies">The replies to the submission.</param>
        internal CommentThreadNavigator(Submission submission, IReadOnlyList<IThing<IHasParent>> replies, SubmissionsCommentSort sort)
            : this(submission, replies, sort, parent: null)
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
        /// <param name="sort">The option the replies are sorted by.</param>
        /// <param name="parent">The parent comment the replies to belong to.</param>
        internal CommentThreadNavigator(
            Submission submission,
            IReadOnlyList<IThing<IHasParent>> replies,
            SubmissionsCommentSort sort,
            Comment parent)
        {
            Requires.NotNull(submission, nameof(submission));
            Requires.NotNull(replies, nameof(replies));

            _submission = submission;
            _sort = sort;
            _parent = parent;
            _comments = replies.OfType<Comment>().ToList();
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
            new CommentThread(_submission, _parent, _sort) :
            null;

        /// <inheritdoc />
        public CommentThread this[int index] => new CommentThread(_submission, _comments[index], _sort);

        /// <inheritdoc />
        public int Count => _comments.Count;

        /// <inheritdoc />
        public IEnumerator<CommentThread> GetEnumerator() =>
            _comments.Select(c => new CommentThread(_submission, c, _sort)).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Creates a flattened representation of all comment threads and their children.
        /// </summary>
        /// <returns>An enumerable representing the flattened comment thread.</returns>
        public IEnumerable<CommentThread> Flatten()
        {
            var comments = new Stack<Comment>(Enumerable.Reverse(_comments));

            while (comments.Any())
            {
                var comment = comments.Pop();
                var replies = comment.Data.Replies?.Children;

                yield return new CommentThread(_submission, comment, _sort);

                if (replies == null)
                {
                    continue;
                }

                foreach (var reply in replies.Reverse())
                {
                    if (reply is Comment)
                    {
                        comments.Push(reply as Comment);
                    }
                }

            }
        }

        /// <summary>
        /// Loads all unresolved comment threads at the current level this navigator manages and adds them to the navigator.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When the comments of a submission are loaded, Reddit does not return the full comment tree and instead includes <i>unresolved</i> comment threads
        /// (represented as things of kind <i>more</i>).
        /// </para>
        /// <para>
        /// A call to this method will load these unresolved comment threads and add them to the set of comments the navigator manages.
        /// </para>
        /// <para>
        /// If a limit is provided, any remaining unresolved comment threads can be loaded via a subsequent call to this method.
        /// </para>
        /// <para>
        /// Note that this method can take a long time to execute as comments must be loaded in sequential batches via the Reddit API.
        /// </para>
        /// <para>
        /// Any unresolved comments in replies to the comments at this level will not be loaded. To load those, first navigate to the replies and then
        /// call this method on the child navigator.
        /// </para>
        /// </remarks>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load unresolved comments.</param>
        /// <param name="limit">A limit on how many unresolved comment threads should be loaded. A value of <see langword="null" /> will cause all to be loaded.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LoadAllCommentsAsync(RedditClient client, int? limit = 20, CancellationToken cancellationToken = default)
        {
            var remaining = limit;
            var context = new LoadAllCommentsContext(_comments, _moreComments);

            // The idea here is to load the children for each 'unresolved comment thread' and attach them to the appropriate parent.
            // Some things to note on this process however:
            //   * Unresolved comment threads are managed in a max-heap, so that threads with the most comments are loaded first.
            //   * More unresolved comment threads may be discovered as we go (i.e. the heap may grow and elements change order)
            //   * A 'context' is used to keep track of all resolved and unresolved comments and the process executes
            while ((remaining == null || remaining > 0) && context.UnresolvedCommentThreads.TryDequeue(out var unresolvedComment, out _))
            {
                var children = await GetChildrenAsync(client, unresolvedComment, cancellationToken).ConfigureAwait(false);

                foreach (var child in children)
                {
                    var parent = FindParent(child, context);

                    AddToParent(parent, child, context);
                }

                remaining -= 1;
            }

            // Add any leftovers to be loaded by a subsequent call.
            _moreComments = context.UnresolvedCommentThreads.UnorderedItems.Select(i => i.Element).ToList();
        }

        private async Task<IReadOnlyList<IThing<IHasParent>>> GetChildrenAsync(RedditClient client, MoreComments unresolvedComment, CancellationToken cancellationToken = default)
        {
            var commentIds = unresolvedComment.Data.Children.ToArray();

            var loadMoreCommentsCommand = new LoadMoreCommentsCommand(new LoadMoreCommentsCommand.Parameters()
            {
                SubmissionId = $"{Constants.Kind.Submission}_{_submission.Data.Id}",
                CommentIds = commentIds,
                Sort = _sort.Name
            });

            var response = await client.ExecuteCommandAsync<JsonDataResponse<MoreCommentsDataNode>>(loadMoreCommentsCommand, cancellationToken);

            return response.Data.Things;
        }

        private IThing<IUserContent> FindParent(IThing<IHasParent> child, LoadAllCommentsContext context)
        {
            if (child is MoreComments)
            {
                // Unresolved comment threads don't have a parent and will instead
                // be added to the collection of comments to resolve.
                return null;
            }

            var kind = child.Data.ParentFullName.Split("_")[0];
            var id = child.Data.ParentFullName.Split("_")[1];

            // Parent is the submission (i.e. child is a new top-level comment)
            if (kind == Constants.Kind.Submission)
            {
                return _submission;
            }

            // Parent is another comment - either a new top-level comment or a reply to another comment.
            if (kind == Constants.Kind.Comment && context.ResolvedComments.TryGetValue(id, out var parent))
            {
                return parent;
            }

            throw new ArgumentException($"Unable to find parent for child with kind '{kind}' and identifier '{id}'", nameof(child));
        }

        private void AddToParent(
            IThing<IUserContent> parent,
            IThing<IHasParent> child,
            LoadAllCommentsContext context)
        {
            switch ((parent, child))
            {
                // An unresolved thread of comments to load.
                case (null, MoreComments unresolvedCommentThread):
                    {
                        if (unresolvedCommentThread.Data.Count == 0)
                        {
                            // This is a "continue this thread" marker - for now just ignore it.
                            // TODO: Support loading these instances too.
                            return;
                        }

                        context.UnresolvedCommentThreads.Enqueue(unresolvedCommentThread, unresolvedCommentThread.Data.Count);
                    }
                    break;

                // A top-level comment.
                case (Submission, Comment resolvedComment):
                    {
                        // Ignore the comment if it has already been resolved before.
                        if (!context.ResolvedComments.ContainsKey(resolvedComment.Data.Id))
                        {
                            _comments.Add(resolvedComment);

                            context.ResolvedComments.Add(resolvedComment.Data.Id, resolvedComment);
                        }
                    }
                    break;

                // A reply to another comment.
                case (Comment comment, Comment reply):
                    {
                        comment.AddCommentToReplies(reply);

                        context.ResolvedComments.Add(reply.Data.Id, reply);
                    }
                    break;
            }
        }

        private class LoadAllCommentsContext
        {
            public LoadAllCommentsContext(List<Comment> comments, List<MoreComments> moreComments)
            {
                UnresolvedCommentThreads = new PriorityQueue<MoreComments, int>(
                    moreComments.Select(mc => (mc, mc.Data.Count)),
                    comparer: new UnloadedCommentComparer());
                ResolvedComments = comments.ToDictionary(c => c.Data.Id);
            }

            public PriorityQueue<MoreComments, int> UnresolvedCommentThreads { get; }
            public Dictionary<string, Comment> ResolvedComments { get; }

            private class UnloadedCommentComparer : IComparer<int>
            {
                public int Compare(int x, int y) => y.CompareTo(x);
            }
        }
    }
}
