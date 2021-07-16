namespace Reddit.NET.Client.Models.Public.Listings.Options
{
    /// <summary>
    /// Represents a sort option for user history
    /// </summary>
    public sealed class UserHistorySort : NamedOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserHistorySort" /> class.
        /// </summary>
        /// <param name="name">The name of the sort option.</param>
        private UserHistorySort(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <i>hot</i> sort option.
        /// </summary>
        public static UserHistorySort Hot => new UserHistorySort("hot");

        /// <summary>
        /// Gets the <i>new</i> sort option.
        /// </summary>
        public static UserHistorySort New => new UserHistorySort("new");

        /// <summary>
        /// Gets the <i>top</i> sort option.
        /// </summary>
        public static UserHistorySort Top => new UserHistorySort("top");

        /// <summary>
        /// Gets the <i>controversial</i> sort option.
        /// </summary>
        public static UserHistorySort Controversial => new UserHistorySort("controversial");
    }
}
