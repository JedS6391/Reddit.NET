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
        /// Gets the <i>all</i> message type.
        /// </summary>
        public static InboxMessageType All => new InboxMessageType("inbox");

        /// <summary>
        /// Gets the <i>inbox</i> message type.
        /// </summary>
        public static InboxMessageType Unread => new InboxMessageType("unread");

        /// <summary>
        /// Gets the <i>sent</i> message type.
        /// </summary>
        public static InboxMessageType Sent => new InboxMessageType("sent");

        /// <summary>
        /// Gets the <i>mentions</i> message type.
        /// </summary>
        public static InboxMessageType Mentions => new InboxMessageType("mentions");

        /// <summary>
        /// Gets the <i>private messages</i> message type.
        /// </summary>
        public static InboxMessageType PrivateMessages => new InboxMessageType("messages");

        /// <summary>
        /// Gets the <i>comment replies</i> message type.
        /// </summary>
        public static InboxMessageType CommentReplies => new InboxMessageType("comments");

        /// <summary>
        /// Gets the <i>submission replies</i> message type.
        /// </summary>
        public static InboxMessageType SubmissionReplies => new InboxMessageType("selfreply");
    }
}
