using Reddit.NET.Client.Interactions.Abstract;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a comment.
    /// </summary>
    public sealed class CommentInteractor : UserContentInteractor, IInteractor
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="CommentInteractor" /> class.
        /// </summary>        
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="commentId">The base-36 ID of the comment to interact with.</param>
        public CommentInteractor(RedditClient client, string commentId)
            : base(client, kind: Constants.Kind.Comment, id: commentId)
        {
        }      
    }
}