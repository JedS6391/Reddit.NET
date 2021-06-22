using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Internal.Json
{
    internal class CommentRepliesJsonConverter : JsonConverter<Listing<IHasParent>>
    {
        public override Listing<IHasParent> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // There are replies to the comment, so we can read a listing.
                return JsonSerializer.Deserialize<Listing<IHasParent>>(ref reader, options);
            }

            if (reader.TokenType == JsonTokenType.String && reader.GetString() == string.Empty)
            {
                // There are no replies to the comment, so just treat it as a null listing.
                // TODO: Is it more appropriate to return an empty listing? 
                return null;
            }

            throw new JsonException("Unexpected JSON data for comment replies.");
        }

        public override void Write(Utf8JsonWriter writer, Listing<IHasParent> value, JsonSerializerOptions options) =>
            throw new NotImplementedException();        
    }
}