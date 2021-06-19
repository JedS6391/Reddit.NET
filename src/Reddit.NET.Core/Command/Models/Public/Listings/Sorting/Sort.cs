namespace Reddit.NET.Core.Client.Command.Models.Public.Listings.Sorting
{
    /// <summary>
    /// Represents a sort option for a <see cref="Abstract.ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation.
    /// </summary>
    public abstract class Sort
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sort" /> class.
        /// </summary>
        /// <param name="name">The name of the sort option.</param>
        protected Sort(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the sort option.
        /// </summary>
        public string Name { get; }
    }
}