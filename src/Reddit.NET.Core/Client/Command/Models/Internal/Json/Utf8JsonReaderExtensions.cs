using System.Text.Json;

namespace Reddit.NET.Core.Client.Command.Models.Internal.Json
{
    /// <summary>
    /// Defines extensions for <see cref="Utf8JsonReader" />.
    /// </summary>
    public static class Utf8JsonReaderExtensions
    {
        /// <summary>
        /// Attempts to match the provided <see cref="JsonTokenType" /> at the current reader position.
        /// </summary>
        /// <param name="reader">The reader to match against.</param>
        /// <param name="tokenType">The type of token to match.</param>
        /// <exception cref="JsonException">Thrown when the current reader token does not match the provided token type.</exception>
        public static void Match(this Utf8JsonReader reader, JsonTokenType tokenType)
        {
            if (reader.TokenType != tokenType)
            {
                FailRead($"Expected '{tokenType}' token but was '{reader.TokenType}'.");
            }
        }

        /// <summary>
        /// Attempts to consume the provided <see cref="JsonTokenType" /> at the current reader position.
        /// </summary>
        /// <param name="reader">The reader to consume from.</param>
        /// <param name="tokenType">The type of token to consume.</param>
        /// <exception cref="JsonException">Thrown when the current reader token does not match the provided token type.</exception>
        public static void Consume(this ref Utf8JsonReader reader, JsonTokenType tokenType) 
        {
            Match(reader, tokenType);

            reader.Read();
        }

        private static void FailRead(string details) => 
            throw new JsonException($"Unexpected JSON data during read: {details}");  
    }
}