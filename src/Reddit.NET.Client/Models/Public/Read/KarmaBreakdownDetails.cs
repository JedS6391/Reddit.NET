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
        /// <param name="karmaList">The karma breakdown data.</param>
        internal KarmaBreakdownDetails(KarmaList.Details karmaList)
        {
            Subreddit = karmaList.Subreddit;
            CommentKarma = karmaList.CommentKarma;
            SubmissionKarma = karmaList.LinkKarma;
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
        public int SubmissionKarma { get; }

        /// <inheritdoc />
        public override string ToString() =>
            $"KarmaBreakdown [Subreddit = {Subreddit}, Comment Karma = {CommentKarma}, Submission Karma = {SubmissionKarma}]";
    }
}
