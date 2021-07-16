namespace Reddit.NET.Client.Models.Public.Listings.Options
{
    /// <summary>
    /// Represents a sort option for submissions in a subreddit.
    /// </summary>
    public sealed class SubredditSubmissionSort : NamedOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditSubmissionSort" /> class.
        /// </summary>
        /// <param name="name">The name of the sort option.</param>
        private SubredditSubmissionSort(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <i>hot</i> sort option.
        /// </summary>
        public static SubredditSubmissionSort Hot => new SubredditSubmissionSort("hot");

        /// <summary>
        /// Gets the <i>best</i> sort option.
        /// </summary>
        public static SubredditSubmissionSort Best => new SubredditSubmissionSort("best");

        /// <summary>
        /// Gets the <i>new</i> sort option.
        /// </summary>
        public static SubredditSubmissionSort New => new SubredditSubmissionSort("new");

        /// <summary>
        /// Gets the <i>rising</i> sort option.
        /// </summary>
        public static SubredditSubmissionSort Rising => new SubredditSubmissionSort("rising");

        /// <summary>
        /// Gets the <i>controversial</i> sort option.
        /// </summary>
        public static SubredditSubmissionSort Controversial => new SubredditSubmissionSort("controversial");

        /// <summary>
        /// Gets the <i>top</i> sort option.
        /// </summary>
        public static SubredditSubmissionSort Top => new SubredditSubmissionSort("top");
    }
}
