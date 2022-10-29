namespace Reddit.NET.Client.Models.Public.Listings.Options
{
    /// <summary>
    /// Represents a sort option for duplicates of a given submission.
    /// </summary>
    public sealed class DuplicateSubmissionSort : NamedOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateSubmissionSort" /> class.
        /// </summary>
        /// <param name="name">The name of the sort option.</param>
        private DuplicateSubmissionSort(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <i>num_comments</i> sort option.
        /// </summary>
        public static DuplicateSubmissionSort NumberOfComments => new DuplicateSubmissionSort("num_comments");

        /// <summary>
        /// Gets the <i>new</i> sort option.
        /// </summary>
        public static DuplicateSubmissionSort New => new DuplicateSubmissionSort("new");
    }
}
