using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Public.Abstract
{
    /// <summary>
    /// Provides mechanisms to asynchronously enumerate over the data contained in a <see cref="Listing{TData}" />.
    /// </summary>
    /// <remarks>
    /// A <see cref="Listing{TData}" /> is how the reddit API supports pagination. 
    /// 
    /// A listing enumerable wraps a reddit API call that returns a listing and provides the ability to 
    /// move through the data in each listing.
    /// 
    /// Listing enumerables operate in a lazy manner, meaning no API calls will be made until enumeration begins.
    /// The enumerator will request a page of data at a time and returned each child in that page before fetching the next page.
    /// </remarks>
    /// <typeparam name="TListing">The type of listing the enumerable manages.</typeparam>
    /// <typeparam name="TData">The type of data associated with the things that this listing contains.</typeparam>
    /// <typeparam name="TMapped">The type the things in the listing will be mapped to before being returned.</typeparam>
    /// <typeparam name="TOptions">The type of options available to the listing.</typeparam>
    public abstract class ListingEnumerable<TListing, TData, TMapped, TOptions> 
        : IAsyncEnumerable<TMapped>
        where TListing : Listing<TData>
        where TOptions : ListingEnumerableOptions, new()        
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> class.
        /// </summary>
        /// <param name="options">The options available to the listing.</param>
        protected ListingEnumerable(TOptions options)
        {
            ListingOptions = options;
        }

        /// <summary>
        /// Gets the options available to the listing.
        /// </summary>
        protected TOptions ListingOptions { get; }

        /// <summary>
        /// Provides the data for the initial listing.
        /// </summary>
        /// <remarks>
        /// This method won't be called until enumeration starts (i.e. the enumerator is lazy).
        /// </remarks>
        /// <returns>A task representing the asynchronous operation. The task result contains the initial listing data.</returns>
        internal abstract Task<TListing> GetInitialListingAsync();

        /// <summary>
        /// Provides the data for the next listing.
        /// </summary>
        /// <remarks>
        /// This method will be called after the data in the current page has been enumerated. 
        /// If the current page indicates no next page (i.e. <see cref="ListingData{TData}.After" /> is <see langword="null" />).
        /// </remarks>
        /// <returns>A task representing the asynchronous operation. The task result contains the next listing data.</returns>
        internal abstract Task<TListing> GetNextListingAsync(TListing currentListing);

        /// <summary>
        /// Maps a <see cref="Thing{TData}" /> entity to an instance of type <typeparamref name="TMapped" />.
        /// </summary>
        /// <param name="thing">The thing to map.</param>
        /// <returns></returns>
        internal abstract TMapped MapThing(IThing<TData> thing);

        /// <inheritdoc />
        public IAsyncEnumerator<TMapped> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
            new ListingEnumerator(
                ListingOptions,
                GetInitialListingAsync, 
                GetNextListingAsync, 
                MapThing,
                cancellationToken);    

        /// <summary>
        /// Supports asynchronous enumeration over <see cref="Listing{TData}" /> data.
        /// </summary>
        /// <remarks>
        /// The enumerator itself only exposes the mapped data.
        /// </remarks>
        internal class ListingEnumerator : IAsyncEnumerator<TMapped>
        {
            private readonly ListingEnumeratorContext _context;

            /// <summary>
            /// Initializes a new instance of the <see cref="ListingEnumerator" /> class.
            /// </summary>
            /// <param name="options">The options available to the listing enumerator.</param>
            /// <param name="initialListingProvider">A function that provides the data for the initial listing.</param>
            /// <param name="nextListingProvider">A function that provides the data for subsequent listings</param>
            /// <param name="mapper">A function used to map the listing data.</param>
            /// <param name="cancellationToken">A <see cref="CancellationToken" /> that may be used to cancel the asynchronous iteration.</param>
            internal ListingEnumerator(
                TOptions options,
                Func<Task<TListing>> initialListingProvider,
                Func<TListing, Task<TListing>> nextListingProvider,
                Func<IThing<TData>, TMapped> mapper,
                CancellationToken cancellationToken = default)
            {
                _context = new ListingEnumeratorContext(
                    options,
                    initialListingProvider,
                    nextListingProvider,
                    mapper,
                    cancellationToken);
            }

            /// <inheritdoc />
            public TMapped Current => _context.Mapper.Invoke(_context.GetCurrentThing);                          

            /// <inheritdoc />
            public async ValueTask<bool> MoveNextAsync()
            {
                _context.CancellationToken.ThrowIfCancellationRequested();

                if (!_context.HasStarted)
                {
                    if (!await TryLoadInitialPageAsync().ConfigureAwait(false))
                    {
                        // No data for the initial page.
                        return false;
                    }
                }         

                if (_context.Options.MaximumItems.HasValue && _context.Count >= _context.Options.MaximumItems)
                {
                    return false;
                }

                _context.MoveNext();

                if (!_context.HasMoreData)
                {                    
                    if (!await TryLoadNextPageAsync().ConfigureAwait(false))
                    {
                        // No data for the next page.
                        return false;
                    }
                }
                                
                return _context.HasMoreData;
            }

            /// <inheritdoc />
            public void Reset()
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;


            private async Task<bool> TryLoadInitialPageAsync()
            {
                if (_context.HasStarted)
                {
                    throw new InvalidOperationException("Cannot load initial listing has a listing has already been loaded.");
                }

                var initialListing = await _context
                    .InitialListingProvider
                    .Invoke()
                    .ConfigureAwait(false);

                if (initialListing == null || !initialListing.Data.Children.Any())
                {
                    return false;
                }

                _context.UpdateListing(initialListing);

                return true;
            }

            private async Task<bool> TryLoadNextPageAsync()
            {
                if (!_context.HasStarted || _context.Exhausted)
                {
                    return false;
                }

                var nextListing = await _context
                    .NextListingProvider
                    .Invoke(_context.CurrentListing)
                    .ConfigureAwait(false);

                if (nextListing == null || !nextListing.Data.Children.Any())
                {
                    return false;
                }

                _context.UpdateListing(nextListing);
                _context.ResetPosition();

                return true;
            }

            private class ListingEnumeratorContext
            {
                public ListingEnumeratorContext(
                    TOptions options,
                    Func<Task<TListing>> initialListingProvider,
                    Func<TListing, Task<TListing>> nextListingProvider,
                    Func<IThing<TData>, TMapped> mapper,
                    CancellationToken cancellationToken)
                {
                    Options = options;
                    InitialListingProvider = initialListingProvider;
                    NextListingProvider = nextListingProvider;
                    Mapper = mapper;
                    CurrentListing = null;
                    Position = -1;
                    Count = 0;
                    Exhausted = false;
                    CancellationToken = cancellationToken;
                }

                public TOptions Options { get; }
                public Func<Task<TListing>> InitialListingProvider { get; }
                public Func<TListing, Task<TListing>> NextListingProvider { get; }
                public Func<IThing<TData>, TMapped> Mapper { get; }
                public TListing CurrentListing { get; private set; }
                public int Position { get; private set; }
                public int Count { get; private set; }
                public bool Exhausted { get; private set; }
                public CancellationToken CancellationToken { get; }

                public bool HasStarted => CurrentListing != null && Position >= 0;
                public bool HasMoreData => Position < CurrentListing.Data.Children.Count;
                public IThing<TData> GetCurrentThing => CurrentListing.Data.Children[Position];

                public void UpdateListing(TListing listing) 
                {
                    Exhausted = 
                        listing.Data.After == CurrentListing?.Data.After ||
                        string.IsNullOrEmpty(listing.Data.After);

                    CurrentListing = listing;                    
                }

                public void MoveNext() 
                {
                    Position++;
                    Count++;;
                }

                public void ResetPosition() => Position = 0;
            }
        }
    }
}