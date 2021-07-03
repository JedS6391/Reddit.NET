using System.Collections.Generic;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Internal
{
    /// <summary>
    /// Defines a container for the karma breakdown of a user.
    /// </summary>
    /// <remarks>
    /// Note that the data type of the thing is a list of details.
    /// </remarks>
    public class KarmaList : Thing<IReadOnlyList<KarmaList.Details>>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="KarmaList" /> entry.
        /// </summary>
        public class Details
        {
            /// <summary>
            /// Gets the name of the subreddit.
            /// </summary>
            [JsonPropertyName("sr")]
            [JsonInclude]
            public string Subreddit { get; private set; }

            /// <summary>
            /// Gets the comment karma.
            /// </summary>
            [JsonPropertyName("comment_karma")]
            [JsonInclude]
            public int CommentKarma { get; private set; }

            /// <summary>
            /// Gets the link karma.
            /// </summary>
            [JsonPropertyName("link_karma")]
            [JsonInclude]
            public int LinkKarma { get; private set; }               
        }
    }
}