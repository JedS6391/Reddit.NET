namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Represents an object that can be voted on. 
    /// </summary>
    /// <see href="https://github.com/reddit-archive/reddit/wiki/JSON#votable-implementation" />
    public interface IVoteable
    {    
        /// <summary>
        /// Gets the number of upvotes.
        /// </summary>
        int Upvotes { get; }
    
        /// <summary>
        /// Gets the number of downvotes.
        /// </summary>
        int Downvotes { get; }
        
        /// <summary>
        /// Gets a value indicating whether the user likes this thing.
        /// </summary>
        /// <remarks>
        /// Will be <see langword="true" /> when liked by the user, <see langword="false" /> if disliked, or
        /// <see langword="null" /> if the user has not voted or there is no authenticated user.
        /// </remarks>
        bool? LikedByUser { get;  }
    }
}