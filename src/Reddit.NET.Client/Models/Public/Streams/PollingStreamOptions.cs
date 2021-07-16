using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reddit.NET.Client.Models.Public.Streams
{
    /// <summary>
    /// Represents the options available to a stream created by the <see cref="PollingStream" /> class.
    /// </summary>
    /// <typeparam name="TData">The type of the underlying data source.</typeparam>
    /// <typeparam name="TMapped">The type to map the data source records to.</typeparam>
    /// <typeparam name="TId">The type of the identifier of the data source records.</typeparam>
    internal class PollingStreamOptions<TData, TMapped, TId>
    {
        private readonly Func<Task<IEnumerable<TData>>> _dataProvider;
        private readonly Func<TData, TMapped> _mapper;
        private readonly Func<TData, TId> _idSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingStreamOptions{TData, TOut, TId}" /> class.
        /// </summary>
        /// <param name="dataProvider">A function that can be invoked to obtain new items from the data source.</param>
        /// <param name="mapper">A function that can be used to map data source records returned by the data provider.</param>
        /// <param name="idSelector">A function that can be used to select the identifier of a data source record.</param>
        public PollingStreamOptions(
            Func<Task<IEnumerable<TData>>> dataProvider,
            Func<TData, TMapped> mapper,
            Func<TData, TId> idSelector)
        {
            _dataProvider = dataProvider;
            _mapper = mapper;
            _idSelector = idSelector;
        }

        /// <summary>
        /// Queries the underlying data source to obtain a new set of records.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains the new data source records.</returns>
        public async Task<IEnumerable<TData>> GetDataAsync() => await _dataProvider.Invoke().ConfigureAwait(false);

        /// <summary>
        /// Maps the provided data source record to an instance of type <typeparamref name="TMapped" />.
        /// </summary>
        /// <param name="data">The data source record to map.</param>
        /// <returns>The mapped data source record.</returns>
        public TMapped MapData(TData data) => _mapper.Invoke(data);

        /// <summary>
        /// Gets the identifier of the provided data source record
        /// </summary>
        /// <param name="data">The data source record to determine the identifier of..</param>
        /// <returns>The identifier of the data source record.</returns>
        public TId GetId(TData data) => _idSelector.Invoke(data);
    }
}
