using System.Text.Json.Serialization;

namespace Reddit.NET.Client.Models.Internal
{
    /// <summary>
    /// Represents a flair available for submissions to a subreddit.
    /// </summary>
    public class SubmissionFlair
    {
        /// <summary>
        /// Gets the identifier of the flair.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; private set; }

        /// <summary>
        /// Gets the text of the flair.
        /// </summary>
        [JsonPropertyName("text")]
        [JsonInclude]
        public string Text { get; private set; }
    }
}
