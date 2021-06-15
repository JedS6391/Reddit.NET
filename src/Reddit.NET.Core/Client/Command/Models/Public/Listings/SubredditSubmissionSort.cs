namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    /// <summary>
    /// Represents a sort order for submissions in a subreddit.
    /// </summary>
    public sealed class SubredditSubmissionSort
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditSubmissionSort" /> class.
        /// </summary>
        /// <param name="name">The name of the sort order.</param>
        private SubredditSubmissionSort(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the sort order.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the 'hot' sort order.
        /// </summary>
        public static SubredditSubmissionSort Hot => new SubredditSubmissionSort("hot");

        /// <summary>
        /// Gets the 'hot' sort order.
        /// </summary>
        public static SubredditSubmissionSort Best => new SubredditSubmissionSort("best");

        /// <summary>
        /// Gets the 'new' sort order.
        /// </summary>
        public static SubredditSubmissionSort New => new SubredditSubmissionSort("new");

        /// <summary>
        /// Gets the 'rising' sort order.
        /// </summary>
        public static SubredditSubmissionSort Rising => new SubredditSubmissionSort("rising");
        
        /// <summary>
        /// Gets the 'controversial' sort order.
        /// </summary>
        public static SubredditSubmissionSort Controversial => new SubredditSubmissionSort("controversial");

        /// <summary>
        /// Gets the 'top' sort order.
        /// </summary>        
        public static SubredditSubmissionSort Top => new SubredditSubmissionSort("top");
    }
}