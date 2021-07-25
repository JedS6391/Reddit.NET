using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Represents a simple JSON data response returned by some reddit API endpoints.
    /// </summary>
    internal class JsonDataResponse<TDataNode>
    {
        /// <summary>
        /// Gets the <c>json</c> node.
        /// </summary>
        [JsonPropertyName("json")]
        [JsonInclude]
        public JsonNode<TDataNode> Json { get; private set; }

        /// <summary>
        /// Gets the <c>data</c> node of the <c>json</c> node.
        /// </summary>
        [JsonIgnore]
        public TDataNode Data => Json.Data;
    }

    /// <summary>
    /// Represents the data of the <see cref="JsonDataResponse{TDataNode}.Json" /> node.
    /// </summary>
    internal class JsonNode<TDataNode>
    {
        /// <summary>
        /// Gets the <c>errors</c> list.
        /// </summary>
        /// <remarks>
        /// Errors are represented as lists, which requires a nested list. An example of an error would be:
        /// <c>["ALREADY_SUB", "This community doesn't allow links to be posted more than once, and this link has already been shared", "url"]</c>
        /// </remarks>
        [JsonPropertyName("errors")]
        [JsonInclude]
        public List<List<string>> Errors { get; private set; }

        /// <summary>
        /// Gets the <c>data</c> node.
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
    /// <code language="json">
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
    internal class CreateSubmissionDataNode
    {
        /// <summary>
        /// Gets the <c>url</c>.
        /// </summary>
        [JsonPropertyName("url")]
        [JsonInclude]
        public string Url { get; private set; }

        /// <summary>
        /// Gets the <c>id</c>.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; private set; }

        /// <summary>
        /// Gets the <c>name</c>.
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
    /// <code language="json">
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
    internal class CreateCommentDataNode
    {
        /// <summary>
        /// Gets the <c>things</c>.
        /// </summary>
        [JsonPropertyName("things")]
        [JsonInclude]
        public IReadOnlyList<IThing<IHasParent>> Things { get; private set; }
    }
}
