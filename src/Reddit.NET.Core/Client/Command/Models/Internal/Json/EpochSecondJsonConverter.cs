using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reddit.NET.Core.Client.Command.Models.Internal.Json
{
    /// <summary>
    /// A <see cref="JsonConverter{T}" /> implementation for converting epoch-second format times to <see cref="DateTimeOffset" /> instances.
    /// </summary>
    /// <remarks>
    /// This converter is only intended to be used with values where the epoch-second value represents a UTC time.
    /// </remarks>
    public class EpochSecondJsonConverter : JsonConverter<DateTimeOffset>
    {
        /// <inheritdoc />
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // {
            //     ...
            //     "created_utc": 1623605729.0,
            //     ...
            // }

            reader.Match(JsonTokenType.Number);

            if (!reader.TryGetDouble(out var value))
            {
                throw new JsonException("Could not read raw epoch-second value as double type.");
            }

            try 
            {                
                return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(value));
            }
            catch (Exception exception)
            {
                throw new JsonException("Could not convert epoch-second value.", exception);
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) =>        
            throw new NotImplementedException();        
    }
}