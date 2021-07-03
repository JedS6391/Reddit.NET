namespace Reddit.NET.Client.Models.Public.Abstract
{
    /// <summary>
    /// Defines the base options available to a <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" />.
    /// </summary>
    public abstract class ListingEnumerableOptions
    {
        private const int DefaultItemsPerRequest = 25;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListingEnumerableOptions" /> class.
        /// </summary>
        protected ListingEnumerableOptions()
        {            
        }

        /// <summary>
        /// Gets the number of items to retrieve per request.
        /// </summary>
        /// <remarks>Defaults to 25.</remarks>
        internal int ItemsPerRequest { get; set; } = DefaultItemsPerRequest;

        /// <summary>
        /// Gets the maximum number of items to enumerate.
        /// </summary>
        /// <remarks>Defaults to no maximum.</remarks>
        internal int? MaximumItems { get; set; }
    }
}