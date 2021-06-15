using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    /// <summary>
    /// Defines a read-only view of a comment.
    /// </summary>
    public class CommentDetails
    {
        internal CommentDetails(Thing<Comment.Details> thing)
        {
            Body = thing.Data.Body;
            Upvotes = thing.Data.Upvotes;
            Downvotes = thing.Data.Downvotes;
        }

        /// <summary>
        /// Gets the body of the comment.
        /// </summary>
        public string Body { get; }     

        /// <summary>
        /// Gets the number of upvotes on the comment.
        /// </summary>
        public int Upvotes { get; }

        /// <summary>
        /// Gets the number of downvotes on the comment.
        /// </summary>
        public int Downvotes { get; }
    }
}