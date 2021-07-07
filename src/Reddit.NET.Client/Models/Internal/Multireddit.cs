using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Internal.Json;

namespace Reddit.NET.Client.Models.Internal
{
    /// <summary>
    /// Represents a multireddit created by a reddit user.
    /// </summary>
    public class Multireddit : Thing<Multireddit.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="Multireddit" />.
        /// </summary>
        public class Details : ICreated
        {
            /// <summary>
            /// Gets the display name of the multireddit.
            /// </summary>
            [JsonPropertyName("display_name")]
            [JsonInclude]
            public string DisplayName { get; private set; }

            /// <summary>
            /// Gets the name of the user that the multireddit belongs to.
            /// </summary>
            [JsonPropertyName("owner")]
            [JsonInclude]
            public string OwnerUsername { get; private set; }

            /// <summary>
            /// Gets the subreddits that the multireddit is comprised of.
            /// </summary>
            [JsonPropertyName("subreddits")]
            [JsonInclude]
            public IReadOnlyList<Subreddit> Subreddits { get; private set; }

            /// <inheritdoc />
            [JsonPropertyName("created_utc")]
            [JsonInclude]
            [JsonConverter(typeof(EpochSecondJsonConverter))]
            public DateTimeOffset CreatedAtUtc { get; private set; }
        }

        /// <summary>
        /// Defines the attributes of a subreddit belong to a <see cref="Multireddit" />.
        /// </summary>
        public class Subreddit
        {
            /// <summary>
            /// Gets the name of the subreddit.
            /// </summary>
            [JsonPropertyName("name")]
            [JsonInclude]
            public string Name { get; private set; }
        }
    }
}
