namespace Reddit.NET.Client.Models.Public.Abstract
{
    /// <summary>
    /// Provides the ability to create <see cref="ListingEnumerableOptions" /> instances.
    /// </summary>
    /// <remarks>
    /// The abstract builder provides mechanisms for configuring properties shared among all options implementations.
    /// </remarks>
    /// <typeparam name="TOptions">The type of options the builder is responsible for.</typeparam>
    /// <typeparam name="TBuilder">The type of the concrete builder.</typeparam>
    public abstract class ListingEnumerableOptionsBuilder<TOptions, TBuilder>
        where TOptions : ListingEnumerableOptions, new()
        where TBuilder : ListingEnumerableOptionsBuilder<TOptions, TBuilder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListingEnumerableOptionsBuilder{TOptions, TBuilder}" /> class.
        /// </summary>
        protected ListingEnumerableOptionsBuilder()
        {
            Options = new TOptions();
        }

        /// <summary>
        /// Gets the concrete builder instance.
        /// </summary>
        protected abstract TBuilder Instance { get; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        protected internal readonly TOptions Options;

        /// <summary>
        /// Sets the number of items to retrieve per request.
        /// </summary>
        /// <param name="limit">The number of items to retrieve per request.</param>
        /// <returns>The updated builder.</returns>
        public TBuilder WithItemsPerRequest(int limit)
        {
            Options.ItemsPerRequest = limit;

            return Instance;
        }

        /// <summary>
        /// Sets the maximum number of items to enumerate.
        /// </summary>
        /// <param name="maximum">The maximum number of items to enumerate.</param>
        /// <returns>The updated builder.</returns>
        public TBuilder WithMaximumItems(int maximum)
        {
            Options.MaximumItems = maximum;

            return Instance;
        }
    }
}
