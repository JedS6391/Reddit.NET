using System;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Internal.Json;

namespace Reddit.NET.Client.Models.Internal
{
    /// <summary>
    /// Represents a reddit user.
    /// </summary>
    public class User : Thing<User.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="User" />.
        /// </summary>
        public class Details : ICreated
        {
            /// <summary>
            /// Gets the identifier of the user.
            /// </summary>
            [JsonPropertyName("id")]
            [JsonInclude]
            public string Id { get; private set; }

            /// <summary>
            /// Gets the username of the user.
            /// </summary>
            [JsonPropertyName("name")]
            [JsonInclude]
            public string Name { get; private set; }

            /// <summary>
            /// Gets the comment karma of the user.
            /// </summary>
            [JsonPropertyName("comment_karma")]
            [JsonInclude]
            public int CommentKarma { get; private set; }

            /// <summary>
            /// Gets the link karma of the user.
            /// </summary>
            [JsonPropertyName("link_karma")]
            [JsonInclude]
            public int LinkKarma { get; private set; }

            /// <inheritdoc />
            [JsonPropertyName("created_utc")]
            [JsonInclude]
            [JsonConverter(typeof(EpochSecondJsonConverter))]
            public DateTimeOffset CreatedAtUtc { get; private set; }
        }
    }
}
