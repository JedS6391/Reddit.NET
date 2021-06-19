namespace Reddit.NET.Core.Client.Command.Models.Public.Listings.Sorting
{
    /// <summary>
    /// Represents a sort option for submissions in a subreddit.
    /// </summary>
    public sealed class SubredditSubmissionSort : Sort
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
        /// Gets the 'hot' sort option.
        /// </summary>
        public static SubredditSubmissionSort Hot => new SubredditSubmissionSort("hot");

        /// <summary>
        /// Gets the 'hot' sort option.
        /// </summary>
        public static SubredditSubmissionSort Best => new SubredditSubmissionSort("best");

        /// <summary>
        /// Gets the 'new' sort option.
        /// </summary>
        public static SubredditSubmissionSort New => new SubredditSubmissionSort("new");

        /// <summary>
        /// Gets the 'rising' sort option.
        /// </summary>
        public static SubredditSubmissionSort Rising => new SubredditSubmissionSort("rising");
        
        /// <summary>
        /// Gets the 'controversial' sort option.
        /// </summary>
        public static SubredditSubmissionSort Controversial => new SubredditSubmissionSort("controversial");

        /// <summary>
        /// Gets the 'top' sort option.
        /// </summary>        
        public static SubredditSubmissionSort Top => new SubredditSubmissionSort("top");
    }
}