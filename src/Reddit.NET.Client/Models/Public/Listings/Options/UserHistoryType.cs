namespace Reddit.NET.Client.Models.Public.Listings.Options
{
    /// <summary>
    /// Represents an option for a type of user history.
    /// </summary>
    public sealed class UserHistoryType : NamedOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserHistoryType" /> class.
        /// </summary>
        /// <param name="name">The name of the type.</param>        
        private UserHistoryType(string name)
            : base(name)
        {            
        }

        /// <summary>
        /// Gets the 'overview' type.
        /// </summary>
        public static UserHistoryType Overview = new UserHistoryType("overview");

        /// <summary>
        /// Gets the 'submitted' type.
        /// </summary>
        public static UserHistoryType Submitted = new UserHistoryType("submitted");

        /// <summary>
        /// Gets the 'comments' type.
        /// </summary>
        public static UserHistoryType Comments = new UserHistoryType("comments");

        /// <summary>
        /// Gets the 'saved' type.
        /// </summary>        
        public static UserHistoryType Saved = new UserHistoryType("saved");

        /// <summary>
        /// Gets the 'upvoted' type.
        /// </summary>
        public static UserHistoryType Upvoted = new UserHistoryType("upvoted");

        /// <summary>
        /// Gets the 'downvoted' type.
        /// </summary>
        public static UserHistoryType Downvoted = new UserHistoryType("downvoted");        
    }
}