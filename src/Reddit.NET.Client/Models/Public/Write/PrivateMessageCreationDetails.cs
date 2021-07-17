using Microsoft;

namespace Reddit.NET.Client.Models.Public.Write
{
    /// <summary>
    /// Represents the details to create a private message to another user.
    /// </summary>
    public class PrivateMessageCreationDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateMessageCreationDetails" /> class.
        /// </summary>
        /// <param name="subject">The subject of the message.</param>
        /// <param name="body">The message content.</param>
        public PrivateMessageCreationDetails(string subject, string body)
        {
            Requires.NotNullOrWhiteSpace(subject, nameof(subject));
            Requires.NotNullOrWhiteSpace(body, nameof(body));

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
