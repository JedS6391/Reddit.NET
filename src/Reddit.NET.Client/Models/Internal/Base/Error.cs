using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Defines the attributes of a reddit API error object.
    /// </summary>
    /// <example>
    /// <code>
    /// {
    ///     "fields": [
    ///         "name"
    ///     ],
    ///     "explanation": "This community name isn't recognizable. Check the spelling and try again.",
    ///     "message": "Bad Request",
    ///     "reason": "BAD_SR_NAME"
    /// }
    /// </code>
    /// </example>
    public class Error
    {
        /// <summary>
        /// Gets the code associated with the error.
        /// </summary>
        /// <remarks>
        /// The reason will be a unique code for the error, e.g. "BAD_SR_NAME"
        /// </remarks>
        [JsonPropertyName("reason")]
        [JsonInclude]
        public string Reason { get; private set; }

        /// <summary>
        /// Gets the message associated with the error.
        /// </summary>
        [JsonPropertyName("message")]
        [JsonInclude]
        public string Message { get; private set; }

        /// <summary>
        /// Gets the explanation associated with the error.
        /// </summary>
        [JsonPropertyName("explanation")]
        [JsonInclude]
        public string Explanation { get; private set; }

        /// <summary>
        /// Gets the fields associated with the error.
        /// </summary>
        [JsonPropertyName("fields")]
        [JsonInclude]
        public IReadOnlyList<string> Fields { get; private set; }
    }
}
