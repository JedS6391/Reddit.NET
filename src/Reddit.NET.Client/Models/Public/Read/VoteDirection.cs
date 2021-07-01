namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines the voting directions for a submission or comment.
    /// </summary>
    public enum VoteDirection
    {
        /// <summary>
        /// Downvoted.
        /// </summary>
        Downvoted = -1,

        /// <summary>
        /// No vote.
        /// </summary>
        NoVote = 0,    

        /// <summary>
        /// Upvoted
        /// </summary>
        Upvoted = 1
    }
}