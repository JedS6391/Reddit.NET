namespace Reddit.NET.Core.Client.Command.Models.Public.Abstract
{
    public abstract class ListingEnumerableOptionsBuilder<TOptions>
        where TOptions : ListingEnumerableOptions
    {
        private readonly TOptions _options;

        protected ListingEnumerableOptionsBuilder(TOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Sets the number of items to retrieve per request.
        /// </summary>
        /// <param name="itemsPerRequest">The number of items to retrieve per request.</param>
        /// <returns>The updated options.</returns>
        public ListingEnumerableOptionsBuilder<TOptions> WithItemsPerRequest(int itemsPerRequest)
        {
            _options.WithItemsPerRequest(itemsPerRequest);

            return this;
        }

        /// <summary>
        /// Sets the maximum number of items to enumerate.
        /// </summary>
        /// <param name="maximum">The maximum number of items to enumerate.</param>
        /// <returns>The updated options.</returns>
        public ListingEnumerableOptionsBuilder<TOptions> WithMaximumItems(int maximum)
        {
            _options.WithMaximumItems(maximum);

            return this;
        }
    }
}