using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Interactions;

namespace Reddit.NET.Client.Models.Public.Read
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
            Vote = thing.Data.LikedByUser switch
            {
                true => VoteDirection.Upvoted,
                false => VoteDirection.Downvoted,
                null => VoteDirection.NoVote
            };            
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
        public CommentInteractor Interact(RedditClient client) => client.Comment(Id);

        /// <inheritdoc />
        public override string ToString() => 
            $"Comment [Subreddit = {Subreddit}, Author = {Author}, Permalink = {Permalink}, CreatedAtUtc = {CreatedAtUtc}]";
    }
}