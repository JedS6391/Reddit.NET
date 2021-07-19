namespace Reddit.NET.Client.Models.Public.Listings.Options
{
    /// <summary>
    /// Represents a sort option for comments on a submission.
    /// </summary>
    public sealed class SubmissionsCommentSort : NamedOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionsCommentSort" /> class.
        /// </summary>
        /// <param name="name">The name of the sort option.</param>
        private SubmissionsCommentSort(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the <i>confidence</i> sort option.
        /// </summary>
        public static SubmissionsCommentSort Confidence => new SubmissionsCommentSort("confidence");

        /// <summary>
        /// Gets the <i>top</i> sort option.
        /// </summary>
        public static SubmissionsCommentSort Top => new SubmissionsCommentSort("top");

        /// <summary>
        /// Gets the <i>new</i> sort option.
        /// </summary>
        public static SubmissionsCommentSort New => new SubmissionsCommentSort("new");

        /// <summary>
        /// Gets the <i>controversial</i> sort option.
        /// </summary>
        public static SubmissionsCommentSort Controversial => new SubmissionsCommentSort("controversial");

        /// <summary>
        /// Gets the <i>old</i> sort option.
        /// </summary>
        public static SubmissionsCommentSort Old => new SubmissionsCommentSort("old");
    }
}
