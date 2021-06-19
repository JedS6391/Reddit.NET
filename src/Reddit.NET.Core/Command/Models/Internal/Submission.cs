using System;
using System.Text.Json.Serialization;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Internal.Json;

namespace Reddit.NET.Core.Client.Command.Models.Internal
{
    /// <summary>
    /// Represents a reddit submission.
    /// </summary>
    public class Submission : Thing<Submission.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="Submission" />.
        /// </summary>
        public class Details : IUserContent
        {
            /// <summary>
            /// Gets the identifier of the submission.
            /// </summary>
            [JsonPropertyName("id")]
            [JsonInclude]
            public string Id { get; private set; }

            /// <summary>
            /// Gets the title of the submission.
            /// </summary>
            [JsonPropertyName("title")]
            [JsonInclude]
            public string Title { get; private set; }

            /// <summary>
            /// Gets the subreddit the submission belongs to.
            /// </summary>
            [JsonPropertyName("subreddit")]
            [JsonInclude]
            public string Subreddit { get; private set; }

            /// <summary>
            /// Gets the permanent link of the submission.
            /// </summary>
            [JsonPropertyName("permalink")]
            [JsonInclude]
            public string Permalink { get; private set; }

            /// <summary>
            /// Gets the URL of the submission
            /// </summary>
            [JsonPropertyName("url")]
            [JsonInclude]
            public string Url { get; private set; }

            /// <summary>
            /// Gets the domain of the submission.
            /// </summary>
            [JsonPropertyName("domain")]
            [JsonInclude]
            public string Domain { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the submission is a self post.
            /// </summary>
            [JsonPropertyName("is_self")]
            [JsonInclude]
            public bool IsSelfPost { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the submission is 'Not Safe For Work' (NSFW).
            /// </summary>
            [JsonPropertyName("over_18")]
            [JsonInclude]
            public bool IsNsfw { get; private set; }

            /// <summary>
            /// Gets the raw text of the submission.
            /// </summary>
            [JsonPropertyName("selftext")]
            [JsonInclude]
            public string SelfText { get; private set; }
            
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
        /// Defines a listing over a collection of <see cref="Submission" /> things.
        /// </summary>
        public class Listing : Listing<Submission.Details> 
        {
        }

        /// <summary>
        /// A custom model used to represent the data returned by a <c>GET /r/{subreddit}/comments/{article}</c> request.
        /// </summary>
        [JsonConverter(typeof(SubmissionCommentsJsonConverter))]
        internal class SubmissionComments
        {
            public Submission.Listing Submissions { get; set;}
            public Comment.Listing Comments { get; set;}
        }
    }
}