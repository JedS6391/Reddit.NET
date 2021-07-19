using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Internal.Json
{
    /// <summary>
    /// A <see cref="JsonConverter{T}" /> implementation for reading JSON data to populate the <see cref="Comment.Details.Replies" /> property.
    /// </summary>
    /// <remarks>
    /// For some reason, when a comment has no replies or the replies aren't loaded, reddit will return an empty string rather than a <c>null</c> value.
    /// </remarks>
    internal class CommentRepliesJsonConverter : JsonConverter<Listing<IHasParent>>
    {
        /// <inheritdoc />
        public override Listing<IHasParent> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.StartObject => JsonSerializer.Deserialize<Listing<IHasParent>>(ref reader, options),
                JsonTokenType.String when reader.GetString() == string.Empty => null,
                _ => throw new JsonException("Unexpected JSON data for comment replies.")
            };

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Listing<IHasParent> value, JsonSerializerOptions options) =>
            throw new NotImplementedException();
    }
}
