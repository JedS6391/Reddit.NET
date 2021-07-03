using System;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Internal.Json;

namespace Reddit.NET.Client.Models.Internal
{
    /// <summary>
    /// Represents a message in a reddit user's inbox.
    /// </summary>
    public class Message : Thing<Message.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="Comment" />.
        /// </summary>
        public class Details : ICreated
        {
            /// <summary>
            /// Gets the author of the message.
            /// </summary>
            [JsonPropertyName("author")]
            [JsonInclude]
            public string Author { get; private set; }

            /// <summary>
            /// Gets the body of the message.
            /// </summary>
            [JsonPropertyName("body")]
            [JsonInclude]
            public string Body { get; private set; }

            /// <inheritdoc />
            [JsonPropertyName("created_utc")]
            [JsonInclude]
            [JsonConverter(typeof(EpochSecondJsonConverter))]
            public DateTimeOffset CreatedAtUtc { get; private set; }
        }

        /// <summary>
        /// Defines a listing over a collection of <see cref="Message" /> things.
        /// </summary>
        public class Listing : Listing<Details>
        { 
        }
    }
}