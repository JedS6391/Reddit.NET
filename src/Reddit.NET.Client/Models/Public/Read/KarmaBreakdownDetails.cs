using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// A read-only view of a user's karma breakdown.
    /// </summary>
    public class KarmaBreakdownDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KarmaBreakdownDetails" /> class.
        /// </summary>
        /// <param name="karmaBreakdown">The karma breakdown data.</param>
        internal KarmaBreakdownDetails(KarmaBreakdown.Details karmaBreakdown)
        {
            Subreddit = karmaBreakdown.Subreddit;
            CommentKarma = karmaBreakdown.CommentKarma;
            SubmissionKarma = karmaBreakdown.LinkKarma;
        }

        /// <summary>
        /// Gets the subreddit where the karma was earned.
        /// </summary>
        public string Subreddit { get; }

        /// <summary>
        /// Gets the karma earned in the subreddit from comments.
        /// </summary>
        public int CommentKarma { get; }

        /// <summary>
        /// Gets the karma earned in the subreddit from submissions.
        /// </summary> 
        public int SubmissionKarma { get;  } 
    }
}