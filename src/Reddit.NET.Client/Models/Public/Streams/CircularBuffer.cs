using System.Linq;

namespace Reddit.NET.Client.Models.Public.Streams
{
    /// <summary>
    /// Represents a buffer with a fixed capacity that will start replacing older items once the capacity is met.
    /// </summary>
    /// <typeparam name="TData">The type of the buffered data.</typeparam>
    internal class CircularBuffer<TData>
    {
        private int _position;
        private readonly int _capacity;
        private readonly TData[] _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBuffer{TData}" /> class.
        /// </summary>
        /// <param name="capacity">The maximum number of items to buffer before older items will be replaced.</param>
        public CircularBuffer(int capacity)
        {
            _position = 0;
            _capacity = capacity;
            _buffer = new TData[capacity];
        }

        /// <summary>
        /// Adds a new item to the buffer.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(TData item)
        {
            _buffer[_position++] = item;

            // We've met the capacity, so cycle back to the beginning of the buffer.
            if (_position >= _capacity)
            {
                _position = 0;
            }
        }

        /// <summary>
        /// Determines whether the provided item is in the buffer.
        /// </summary>
        /// <param name="item">The item to find in the buffer.</param>
        /// <returns><see langword="true" /> if the item is found in the buffer; <see langword="false" /> otherwise.</returns>
        public bool Contains(TData item) => _buffer.Contains(item);
    }
}
