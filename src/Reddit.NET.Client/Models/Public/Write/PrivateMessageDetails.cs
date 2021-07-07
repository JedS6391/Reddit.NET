namespace Reddit.NET.Client.Models.Public.Write
{
    /// <summary>
    /// Represents the details to send a message to a user.
    /// </summary>
    public class PrivateMessageDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateMessageDetails" /> class.
        /// </summary>
        /// <param name="subject">The subject of the message.</param>
        /// <param name="body">The message content.</param>
        public PrivateMessageDetails(string subject, string body)
        {
            Subject = subject;
            Body = body;
        }

        /// <summary>
        /// Gets the subject of the message.
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// Gets the message content.
        /// </summary>
        public string Body { get; }
    }
}
