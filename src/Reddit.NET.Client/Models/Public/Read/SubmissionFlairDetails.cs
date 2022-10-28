using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a flair available for submissions to a subreddit.
    /// </summary>
    public class SubmissionFlairDetails
    {
        internal SubmissionFlairDetails(SubmissionFlair submissionFlair)
        {
            Id = submissionFlair.Id;
            Text = submissionFlair.Text;
        }

        /// <summary>
        /// Gets the identifier of the flair.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the text of the flair.
        /// </summary>
        public string Text { get; }
    }
}
