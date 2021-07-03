using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Internal.Json;

namespace Reddit.NET.Client.Models.Internal
{
    /// <summary>
    /// Represents a trophy awarded to a reddit user.
    /// </summary>
    public class Trophy : Thing<Trophy.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="Trophy" />.
        /// </summary>
        public class Details
        {
            /// <summary>
            /// Gets the name of the trophy.
            /// </summary>
            [JsonPropertyName("name")]
            [JsonInclude]
            public string Name { get; private set; }

            /// <summary>
            /// Gets the description of the trophy.
            /// </summary>
            [JsonPropertyName("description")]
            [JsonInclude]
            public string Description { get; private set; }

            /// <summary>
            /// Gets the URL of the 41x41px icon for the trophy.
            /// </summary>
            [JsonPropertyName("icon_40")]
            [JsonInclude]
            public string Icon40Url { get; private set; }

            /// <summary>
            /// Gets the URL of the 71x71px icon for the trophy.
            /// </summary>
            [JsonPropertyName("icon_70")]
            [JsonInclude]
            public string Icon70Url { get; private set; }

            /// <summary>
            /// Gets the time the trophy was awarded in UTC.
            /// </summary>
            [JsonPropertyName("granted_at")]
            [JsonInclude]
            [JsonConverter(typeof(EpochSecondJsonConverter))]
            public DateTimeOffset? AwardedAtUtc { get; private set; }            
        }
    }

    /// <summary>
    /// Defines a container for the trophies awarded to a reddit user.
    /// </summary>
    public class TrophyList : Thing<TrophyList.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="TrophyList" />.
        /// </summary>
        public class Details
        {
            /// <summary>
            /// Gets the list of trophies.
            /// </summary>
            [JsonPropertyName("trophies")]
            [JsonInclude]
            public IReadOnlyList<Trophy> Trophies { get; private set; }
        }
    }
}