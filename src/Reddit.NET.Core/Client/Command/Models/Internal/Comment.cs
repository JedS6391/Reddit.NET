using System;
using System.Text.Json.Serialization;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Internal.Json;

namespace Reddit.NET.Core.Client.Command.Models.Internal
{
    /// <summary>
    /// Represents a comment on a reddit submission.
    /// </summary>
    public class Comment : Thing<Comment.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="Comment" />.
        /// </summary>
        public class Details : IUserContent
        {
            /// <summary>
            /// Gets the identifier of the comment.
            /// </summary>
            [JsonPropertyName("id")]
            [JsonInclude]
            public string Id { get; private set; }

            /// <summary>
            /// Gets the body of the comment.
            /// </summary>
            [JsonPropertyName("body")]
            [JsonInclude]
            public string Body { get; private set; }

            /// <summary>
            /// Gets the subreddit the comment belongs to.
            /// </summary>
            [JsonPropertyName("subreddit")]
            [JsonInclude]
            public string Subreddit { get; private set; }

            /// <summary>
            /// Gets the permanent link of the comment.
            /// </summary>
            [JsonPropertyName("permalink")]
            [JsonInclude]
            public string Permalink { get; private set; }            

            /// <inheritdoc />
            [JsonPropertyName("author")]
            [JsonInclude]
            public string Author { get; private set; }

            /// <inheritdoc />
            [JsonPropertyName("ups")]
            [JsonInclude]
            public int Upvotes { get; private set; }

            /// <inheritdoc />
            [JsonPropertyName("downs")]
            [JsonInclude]
            public int Downvotes { get; private set; }

            /// <inheritdoc />
            [JsonPropertyName("likes")]
            [JsonInclude]
            public bool? LikedByUser { get; private set; }

            /// <inheritdoc />
            [JsonPropertyName("created_utc")]
            [JsonInclude]
            [JsonConverter(typeof(EpochSecondJsonConverter))]
            public DateTimeOffset CreatedAtUtc { get; private set; }            
        }
        
        /// <summary>
        /// Defines a listing over a collection of <see cref="Comment" /> things.
        /// </summary>
        public class Listing : Listing<Comment.Details> 
        {
        }        
    }
}