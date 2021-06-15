namespace Reddit.NET.Core.Client.Command.Models.Public.Abstract
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
        internal int ItemsPerRequest { get; private set; } = DefaultItemsPerRequest;

        /// <summary>
        /// Gets the maximum number of items to enumerate.
        /// </summary>
        internal int? MaximumItems { get; private set; }

        /// <summary>
        /// Sets the number of items to retrieve per request.
        /// </summary>
        /// <param name="itemsPerRequest">The number of items to retrieve per request.</param>
        /// <returns>The updated options.</returns>
        public ListingEnumerableOptions WithItemsPerRequest(int itemsPerRequest)
        {
            ItemsPerRequest = itemsPerRequest;

            return this;
        }

        /// <summary>
        /// Sets the maximum number of items to enumerate.
        /// </summary>
        /// <param name="maximum">The maximum number of items to enumerate.</param>
        /// <returns>The updated options.</returns>
        public ListingEnumerableOptions WithMaximumItems(int maximum)
        {
            MaximumItems = maximum;

            return this;
        }
    }
}