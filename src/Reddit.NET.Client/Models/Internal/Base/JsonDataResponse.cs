using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Represents a simple JSON data response returned by some reddit API endpoints.
    /// </summary>
    public class JsonDataResponse<TDataNode>
    {    
        /// <summary>
        /// Gets the 'json' node.
        /// </summary>
        [JsonPropertyName("json")]
        [JsonInclude]
        public JsonNode<TDataNode> Json { get; private set; }

        /// <summary>
        /// Gets the 'data' node of the 'json' node.
        /// </summary>
        [JsonIgnore]
        public TDataNode Data => Json.Data;
    }

    /// <summary>
    /// Represents the data of the <see cref="JsonDataResponse{TDataNode}.Json" /> node.
    /// </summary>
    public class JsonNode<TDataNode>
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
        public TDataNode Data { get; private set; }
    }

    /// <summary>
    /// Represents the data of the <see cref="JsonNode{TDataNode}.Data" /> node for the create submission endpoint.
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
    public class CreateSubmissionDataNode
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

    /// <summary>
    /// Represents the data of the <see cref="JsonNode{TDataNode}.Data" /> node for the create comment endpoint.
    /// </summary>
    /// <example>
    /// An example of the data returned by the subreddit submission endpoint:
    /// <code>
    /// {
    ///     "json": {
    ///         "errors" : [],
    ///         "data": {
    ///             "things": [
    ///                 {
    ///                     "kind": "t1",
    ///                     "data": {
    ///                         // Comment thing data
    ///                         ...
    ///                     }
    ///                 }
    ///             ]
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public class CreateCommentDataNode  
    {
        /// <summary>
        /// Gets the 'things'.
        /// </summary>
        [JsonPropertyName("things")]
        [JsonInclude]
        public IReadOnlyList<Comment> Things { get; private set; }
    }
}