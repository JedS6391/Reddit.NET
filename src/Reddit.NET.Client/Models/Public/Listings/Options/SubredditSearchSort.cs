namespace Reddit.NET.Client.Models.Public.Listings.Options
{
    /// <summary>
    /// Represents a sort option for searching submissions in a subreddit.
    /// </summary>
    public sealed class SubredditSearchSort : NamedOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditSearchSort" /> class.
        /// </summary>
        /// <param name="name">The name of the sort option.</param>
        private SubredditSearchSort(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <i>relevance</i> sort option.
        /// </summary>
        public static SubredditSearchSort Relevance => new SubredditSearchSort("relevance");

        /// <summary>
        /// Gets the <i>hot</i> sort option.
        /// </summary>
        public static SubredditSearchSort Hot => new SubredditSearchSort("hot");

        /// <summary>
        /// Gets the <i>top</i> sort option.
        /// </summary>
        public static SubredditSearchSort Top => new SubredditSearchSort("top");

        /// <summary>
        /// Gets the <i>new</i> sort option.
        /// </summary>
        public static SubredditSearchSort New => new SubredditSearchSort("new");

        /// <summary>
        /// Gets the <i>comments</i> sort option.
        /// </summary>
        public static SubredditSearchSort Comments => new SubredditSearchSort("comments");
    }
}
