using System.Text.Json.Serialization;

namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Provides a base implementation of <see cref="IThing{TData}" />.
    /// </summary>
    /// <typeparam name="TData">The type of data this kind of thing contains.</typeparam>
    public abstract class Thing<TData> : IThing<TData>
    {
        /// <inheritdoc/>
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; private set; }

        /// <inheritdoc/>
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; private set; }

        /// <inheritdoc/>
        [JsonPropertyName("kind")]
        [JsonInclude]
        public string Kind { get; private set; }

        /// <inheritdoc/>
        [JsonPropertyName("data")]
        [JsonInclude]
        public TData Data { get; private set; }
    }
}
