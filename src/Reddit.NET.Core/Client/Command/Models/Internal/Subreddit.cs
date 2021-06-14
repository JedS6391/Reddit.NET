using System.Text.Json.Serialization;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Internal
{
    /// <summary>
    /// Represents a reddit subreddit.
    /// </summary>
    public class Subreddit : Thing<Subreddit.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="Subreddit" />.
        /// </summary>
        public class Details
        {            
            /// <summary>
            /// Gets the identifier of the subreddit.
            /// </summary>
            [JsonPropertyName("id")]
            [JsonInclude]
            public string Id { get; private set; }         
        
            /// <summary>
            /// Gets the title of the subreddit.
            /// </summary>
            [JsonPropertyName("title")]
            [JsonInclude]
            public string Title { get; private set; }

            /// <summary>
            /// Gets the display name of the subreddit.
            /// </summary>
            [JsonPropertyName("display_name")]
            [JsonInclude]
            public string DisplayName { get; private set; }
        }

        /// <summary>
        /// Defines a listing over a collection of <see cref="Subreddit" /> things.
        /// </summary>
        public class Listing : Listing<Subreddit.Details> 
        {
        }
    }
}