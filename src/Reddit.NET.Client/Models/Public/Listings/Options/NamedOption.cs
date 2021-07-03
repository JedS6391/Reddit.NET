namespace Reddit.NET.Client.Models.Public.Listings.Options
{
    /// <summary>
    /// Represents a named option for a <see cref="Abstract.ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation.
    /// </summary>
    public abstract class NamedOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedOption" /> class.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        protected NamedOption(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the option.
        /// </summary>
        public string Name { get; }
    }
}