using System;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a comment.
    /// </summary>
    public class MessageDetails
    {
        internal MessageDetails(IThing<Message.Details> thing)
        {
            Author = thing.Data.Author;
            Body = thing.Data.Body;
            CreatedAtUtc = thing.Data.CreatedAtUtc;
            Id = thing.Data.Id;
        }

        /// <summary>
        /// Gets the author of the message.
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// Gets the body of the message.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Gets the date and time the message was created.
        /// </summary>
        public DateTimeOffset CreatedAtUtc { get; private set; }

        /// <summary>
        /// Gets the identifier of the message.
        /// </summary>
        /// <see href="https://www.reddit.com/dev/api/oauth/" />
        internal string Id { get; private set; }

        /// <summary>
        /// Gets the full name of the message.
        /// </summary>
        /// <see href="https://www.reddit.com/dev/api/oauth/#fullnames" />
        internal string FullName => $"{Constants.Kind.Message}_{Id}";

        /// <inheritdoc />
        public override string ToString() =>
            $"Message [Author = {Author}, Body = {Body}, CreatedAtUtc = {CreatedAtUtc}]";
    }
}
