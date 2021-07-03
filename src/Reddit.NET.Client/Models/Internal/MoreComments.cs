using System.Collections.Generic;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Internal
{
    /// <summary>
    /// Represents more comments on a reddit submission.
    /// </summary>
    public class MoreComments : Thing<MoreComments.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="MoreComments" />.
        /// </summary>
        public class Details : IHasParent
        {
            /// <summary>
            /// Gets the identifier of the first child in the list of children.
            /// </summary>
            [JsonPropertyName("id")]
            [JsonInclude]
            public string Id { get; private set; }  

            /// <inheritdoc />
            [JsonPropertyName("parent_id")]
            [JsonInclude]
            public string ParentFullName { get; private set; }            

            /// <summary>
            /// Gets the number of children.
            /// </summary>
            [JsonPropertyName("count")]
            [JsonInclude]          
            public int Count { get; private set; }

            /// <summary>
            /// Gets a collection containing identifiers of the children.
            /// </summary>
            [JsonPropertyName("children")]
            [JsonInclude]
            public IReadOnlyList<string> Children { get; private set; }
        }

        /// <summary>
        /// Defines a listing over a collection of <see cref="MoreComments" /> things.
        /// </summary>
        public class Listing : Listing<Details>
        {}
    }
}