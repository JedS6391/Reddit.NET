namespace Reddit.NET.Client.Models.Public.Listings.Options
{
    /// <summary>
    /// Represents an option for a type of inbox message.
    /// </summary>
    public sealed class InboxMessageType : NamedOption
    {
        private InboxMessageType(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets the 'all' message type.
        /// </summary>
        public static InboxMessageType All => new InboxMessageType("inbox");

        /// <summary>
        /// Gets the 'inbox' message type.
        /// </summary>
        public static InboxMessageType Unread => new InboxMessageType("unread");

        /// <summary>
        /// Gets the 'sent' message type.
        /// </summary>
        public static InboxMessageType Sent => new InboxMessageType("sent");

        /// <summary>
        /// Gets the 'mentions' message type.
        /// </summary>
        public static InboxMessageType Mentions => new InboxMessageType("mentions");

        /// <summary>
        /// Gets the 'private messages' message type.
        /// </summary>
        public static InboxMessageType PrivateMessages => new InboxMessageType("messages");

        /// <summary>
        /// Gets the 'comment replies' message type.
        /// </summary>
        public static InboxMessageType CommentReplies => new InboxMessageType("comments");

        /// <summary>
        /// Gets the 'submission replies' message type.
        /// </summary>
        public static InboxMessageType SubmissionReplies => new InboxMessageType("selfreply");
    }
}
