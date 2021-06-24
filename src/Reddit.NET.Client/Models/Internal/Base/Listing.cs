using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reddit.NET.Client.Models.Internal.Base
{
    /// <summary>
    /// Defines the attributes of a reddit API object that supports pagination.
    /// </summary>
    /// <remarks>
    /// This class acts as a container for the properties of a listing, as a listing is 
    /// not a thing so the attributes cannot be shared.
    /// </remarks>
    /// <see href="https://github.com/reddit-archive/reddit/wiki/JSON#listing" />
    /// <typeparam name="TData">The type of data associated with the things that this listing contains.</typeparam>
    public class Listing<TData>
    {
        /// <summary>
        /// Gets the kind of the listing. Should always be <c>Listing</c>.
        /// </summary>
        [JsonPropertyName("kind")]
        [JsonInclude]
        public string Kind { get; private set; }

        /// <summary>
        /// Gets the data of the listing.
        /// </summary>
        [JsonPropertyName("data")]
        [JsonInclude]
        public ListingData<TData> Data { get; private set; }

        /// <summary>
        /// Gets the children of the listing.
        /// </summary>
        /// <remarks>
        /// This is provided as a convenience method to avoid an indirection.
        /// </remarks>
        [JsonIgnore]
        public IReadOnlyList<IThing<TData>> Children => Data?.Children;
    }

    /// <summary>
    /// Defines the data associated with a <see cref="Listing{TData}" /> entity.
    /// </summary>
    /// <typeparam name="TData">The type of data associated with the things that this listing contains.</typeparam>
    public class ListingData<TData>
    {
        /// <summary>
        /// Gets the name of the listing before this listing (i.e. the previous page).
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null" /> if there is no previous page.
        /// </remarks>
        [JsonPropertyName("before")]
        [JsonInclude]
        public string Before { get; private set; }

        /// <summary>
        /// Gets the name of the listing after this listing (i.e. the next page).
        /// </summary>
        /// <remarks>
        /// Will be <see langword="null" /> if there is no next page.
        /// </remarks>
        [JsonPropertyName("after")]
        [JsonInclude]
        public string After { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="Thing{TData}" /> entities this listing contains.
        /// </summary>
        [JsonPropertyName("children")]
        [JsonInclude]
        public IReadOnlyList<IThing<TData>> Children { get; private set; }
    }
}