using System.Text.Json.Serialization;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Internal
{
    /// <summary>
    /// Represents a reddit user.
    /// </summary>
    public class User : Thing<User.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="User" />.
        /// </summary>
        public class Details
        {
            /// <summary>
            /// Gets the identifier of the user.
            /// </summary>
            [JsonPropertyName("id")]
            [JsonInclude]
            public string Id { get; private set; }

            /// <summary>
            /// Gets the username of the user.
            /// </summary>
            [JsonPropertyName("name")]
            [JsonInclude]
            public string Name { get; private set; }
        }
    }
}