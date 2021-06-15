namespace Reddit.NET.Core.Client.Command.Models.Public.Abstract
{
    public abstract class ListingEnumerableOptionsBuilder<TOptions, TBuilder> 
        where TOptions : ListingEnumerableOptions, new()
        where TBuilder : ListingEnumerableOptionsBuilder<TOptions, TBuilder>
    {
        protected ListingEnumerableOptionsBuilder()
        {
            Options = new TOptions();
        }

        protected abstract TBuilder Instance { get; }

        protected internal readonly TOptions Options;

        /// <summary>
        /// Sets the number of items to retrieve per request.
        /// </summary>
        /// <param name="limit">The number of items to retrieve per request.</param>
        /// <returns>The updated options.</returns>
        public TBuilder WithItemsPerRequest(int limit)
        {
            Options.ItemsPerRequest = limit;

            return Instance;
        }

        /// <summary>
        /// Sets the maximum number of items to enumerate.
        /// </summary>
        /// <param name="maximum">The maximum number of items to enumerate.</param>
        /// <returns>The updated options.</returns>
        public TBuilder WithMaximumItems(int maximum)
        {
            Options.MaximumItems = maximum;

            return Instance;
        }
    }
}