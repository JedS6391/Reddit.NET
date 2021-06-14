using System.Text.Json.Serialization;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Internal
{
    /// <summary>
    /// Represents a comment on a reddit submission.
    /// </summary>
    public class Comment : Thing<Comment.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="Comment" />.
        /// </summary>
        public class Details 
        {
            /// <summary>
            /// Gets the identifier of the comment.
            /// </summary>
            [JsonPropertyName("id")]
            [JsonInclude]
            public string Id { get; private set; }

            /// <summary>
            /// Gets the body of the comment.
            /// </summary>
            [JsonPropertyName("body")]
            [JsonInclude]
            public string Body { get; private set; }
        }
        
        /// <summary>
        /// Defines a listing over a collection of <see cref="Comment" /> things.
        /// </summary>
        public class Listing : Listing<Comment.Details> 
        {
        }        
    }
}