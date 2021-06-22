namespace Reddit.NET.Client.Models.Public.Listings.Sorting
{
    /// <summary>
    /// Represents a sort option for user history
    /// </summary>
    public sealed class UserHistorySort : Sort
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
        /// Gets the 'hot' sort option.
        /// </summary>
        public static UserHistorySort Hot => new UserHistorySort("hot");

        /// <summary>
        /// Gets the 'new' sort option.
        /// </summary>
        public static UserHistorySort New => new UserHistorySort("new");

        /// <summary>
        /// Gets the 'top' sort option.
        /// </summary>
        public static UserHistorySort Top => new UserHistorySort("top");

        /// <summary>
        /// Gets the 'controversial' sort option.
        /// </summary>
        public static UserHistorySort Controversial => new UserHistorySort("controversial");        
    }
}