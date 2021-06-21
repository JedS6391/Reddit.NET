using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Represents a simple JSON data response returned by some reddit API endpoints.
    /// </summary>
    /// <example>
    /// An example of the data returned by the subreddit submission endpoint:
    /// <code>
    /// {
    ///     "json": {
    ///         "errors": [],
    ///         "data": {
    ///             "url": "https://www.reddit.com/r/{subreddit}/comments/{id}/{slug}/.json",
    ///             "drafts_count": 0,
    ///             "id": "{id}",
    ///             "name": "{kind}_{id}"
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public class JsonDataResponse
    {    
        /// <summary>
        /// Gets the 'json' node.
        /// </summary>
        [JsonPropertyName("json")]
        [JsonInclude]
        public JsonNode Json { get; private set; }

        /// <summary>
        /// Represents the data of the <see cref="Json" /> node.
        /// </summary>
        public class JsonNode
        {
            // TODO: Need to verify this type
            /// <summary>
            /// Gets the 'errors' list.
            /// </summary>
            [JsonPropertyName("errors")]
            [JsonInclude]
            public List<object> Errors { get; private set; }

            /// <summary>
            /// Gets the 'data' node.
            /// </summary>
            [JsonPropertyName("data")]
            [JsonInclude]
            public DataNode Data { get; private set; }
        }

        /// <summary>
        /// Represents the data of the <see cref="JsonNode.Data" /> node.
        /// </summary>
        public class DataNode
        {
            /// <summary>
            /// Gets the 'url'.
            /// </summary>
            [JsonPropertyName("url")]
            [JsonInclude]
            public string Url { get; private set; }

            /// <summary>
            /// Gets the 'id'.
            /// </summary>
            [JsonPropertyName("id")]
            [JsonInclude]
            public string Id { get; private set; }            

            /// <summary>
            /// Gets the 'name'.
            /// </summary>
            [JsonPropertyName("name")]
            [JsonInclude]
            public string Name { get; private set; }
        }
    }
}