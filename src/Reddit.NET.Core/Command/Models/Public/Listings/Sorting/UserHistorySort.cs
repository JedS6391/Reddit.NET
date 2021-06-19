namespace Reddit.NET.Core.Client.Command.Models.Public.Listings.Sorting
{
    /// <summary>
    /// Represents a sort option for the history of a user.
    /// </summary>
    public sealed class UserHistorySort : Sort
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubredditSubmissionSort" /> class.
        /// </summary>
        /// <param name="name">The name of the sort option.</param>        
        private UserHistorySort(string name)
            : base(name)
        {            
        }

        /// <summary>
        /// Gets the 'overview' sort option.
        /// </summary>
        public static UserHistorySort Overview = new UserHistorySort("overview");

        /// <summary>
        /// Gets the 'submitted' sort option.
        /// </summary>
        public static UserHistorySort Submitted = new UserHistorySort("submitted");

        /// <summary>
        /// Gets the 'comments' sort option.
        /// </summary>
        public static UserHistorySort Comments = new UserHistorySort("comments");

        /// <summary>
        /// Gets the 'saved' sort option.
        /// </summary>        
        public static UserHistorySort Saved = new UserHistorySort("saved");
        
        // TODO: Other sorts
    }
}