using System.Text.Json.Serialization;

namespace Reddit.NET.Client.Models.Internal
{
    /// <summary>
    /// Represents the attributes of a reddit OAuth token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        [JsonPropertyName("access_token")]
        [JsonInclude]
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        [JsonInclude]
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Gets the type of token.
        /// </summary>
        [JsonPropertyName("token_type")]
        [JsonInclude]
        public string TokenType { get; private set; }

        /// <summary>
        /// Gets the number of seconds until <see cref="AccessToken" /> expires.
        /// </summary>
        [JsonPropertyName("expires_in")]
        [JsonInclude]
        public int ExpiresIn { get; private set; }

        /// <summary>
        /// Gets the scope(s) access is granted for.
        /// </summary>
        [JsonPropertyName("scope")]
        [JsonInclude]
        public string Scope { get; private set; }
    }
}
