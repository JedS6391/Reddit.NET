using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Public
{
    public abstract class ListingGenerator<TListing, TData, TMapped> : IAsyncEnumerable<TMapped>
        where TListing : Listing<TData>
    {
        protected ListingGenerator()
        {            
        }

        internal abstract Task<TListing> GetInitialListingAsync();

        internal abstract Task<TListing> GetNextListingAsync(TListing currentListing);

        internal abstract TMapped MapData(TData data);


        public IAsyncEnumerator<TMapped> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new ListingEnumerator(GetInitialListingAsync, GetNextListingAsync, MapData);
        }

        public class ListingEnumerator : IAsyncEnumerator<TMapped>
        {
            private ListingEnumeratorContext _context;

            public ListingEnumerator(
                Func<Task<TListing>> initialListingProvider,
                Func<TListing, Task<TListing>> nextListingProvider,
                Func<TData, TMapped> mapper)
            {
                _context = new ListingEnumeratorContext
                {
                    CurrentListing = null,
                    InitialListingProvider = initialListingProvider,
                    NextListingProvider = nextListingProvider,
                    Mapper = mapper,
                    Position = -1,
                    Exhausted = false
                };
            }

            public TMapped Current
            {
                get 
                {
                    return _context.Mapper.Invoke(_context.CurrentListing.Data.Children[_context.Position].Data);
                }          
            }

            public async ValueTask<bool> MoveNextAsync()
            {                
                await EnsureInitialListingIsFetchedAsync();

                _context.Position++;

                if (_context.Position >= _context.CurrentListing.Data.Children.Count)
                {                    
                    await FetchNextPageAsync();
                }

                return _context.Position < _context.CurrentListing.Data.Children.Count;
             }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;


            private async Task EnsureInitialListingIsFetchedAsync()
            {
                if (_context.CurrentListing == null)
                {
                    _context.CurrentListing = await _context.InitialListingProvider.Invoke().ConfigureAwait(false);
                }
            }

            private async Task FetchNextPageAsync()
            {
                if (_context.Exhausted)
                {
                    return;
                }

                var nextListing = await _context.NextListingProvider.Invoke(_context.CurrentListing).ConfigureAwait(false);

                if (nextListing == null)
                {
                    return;
                }

                _context.Exhausted =
                    nextListing.Data.After == _context.CurrentListing.Data.After ||
                    string.IsNullOrEmpty(nextListing.Data.After);
                _context.CurrentListing = nextListing;
                _context.Position = 0;
            }

            private class ListingEnumeratorContext
            {
                public TListing CurrentListing { get; set; }
                public Func<Task<TListing>> InitialListingProvider { get; set; }
                public Func<TListing, Task<TListing>> NextListingProvider { get; set; }
                public Func<TData, TMapped> Mapper { get;set; }
                public int Position { get; set; }
                public bool Exhausted { get; set; }
            }
        }
    }
}