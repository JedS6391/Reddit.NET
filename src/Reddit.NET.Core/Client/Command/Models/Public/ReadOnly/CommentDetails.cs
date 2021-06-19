using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    /// <summary>
    /// Defines a read-only view of a comment.
    /// </summary>
    public class CommentDetails : UserContentDetails, IToInteractor<CommentInteractor>
    {
        internal CommentDetails(IThing<Comment.Details> thing)
            : base(thing.Kind, thing.Data.Id)
        {
            Body = thing.Data.Body;
            Subreddit = thing.Data.Subreddit;
            Permalink = thing.Data.Permalink;
            Author = thing.Data.Author;
            Upvotes = thing.Data.Upvotes;
            Downvotes = thing.Data.Downvotes;
            CreatedAtUtc = thing.Data.CreatedAtUtc;
        }

        /// <summary>
        /// Gets the body of the comment.
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Gets the subreddit the comment belongs to.
        /// </summary>
        public string Subreddit { get; }

        /// <summary>
        /// Gets the permanent link of the comment.
        /// </summary>
        public string Permalink { get; }

        /// <inheritdoc />
        public CommentInteractor Interact(RedditClient client) => client.Comment(this);

        /// <inheritdoc />
        public override string ToString() => 
            $"Comment [Subreddit = {Subreddit}, Author = {Author}, Permalink = {Permalink}, CreatedAtUtc = {CreatedAtUtc}]";
    }
}