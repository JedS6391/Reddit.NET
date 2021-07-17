using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Reddit.NET.Client.Models.Public.Streams
{
    /// <summary>
    /// Provides the ability to create streams of data that will poll at a regular interval for new data.
    /// </summary>
    internal static class PollingStream
    {
        private static readonly Func<int, TimeSpan> s_sleepDurationProvider =
            (queryCount) => TimeSpan.FromSeconds(Math.Pow(2, Math.Min(queryCount, 16)));

        /// <summary>
        /// Creates a stream that will asynchronously yield new data as it becomes available.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The enumerator will keep track of a set of *seen* identifiers to determine when new data is available.
        /// </para>
        /// <para>
        /// To avoid querying the data provider too frequently, the enumerator will pause using an exponential back-off
        /// strategy based on the number of queries made without new data.
        /// </para>
        /// </remarks>
        /// <typeparam name="TData">The type of the data source.</typeparam>
        /// <typeparam name="TOut">The type to map the raw data source records to.</typeparam>
        /// <typeparam name="TId">The type of the identifier of the data source records.</typeparam>
        /// <param name="options">The options available to the stream.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous iteration.</param>
        /// <returns>An asynchronous enumerator over new data as it becomes available.</returns>
        public static async IAsyncEnumerable<TOut> Create<TData, TOut, TId>(
            PollingStreamOptions<TData, TOut, TId> options,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var queriesWithoutNewData = 0;
            var seenIds = new CircularBuffer<TId>(capacity: 500);

            // The basic idea here is as follows:
            //   - Retrieve data from source
            //   - Determine the records with IDs that not have been seen before
            //   - Return each unseen record and record the ID as seen
            //   - Sleep for some duration of time
            //
            // The sleep duration will be determined based on how many queries (i.e. step 1 above)
            // have been made without any new records identified.
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var hasNewData = false;
                var data = await options.GetDataAsync(cancellationToken).ConfigureAwait(false);

                foreach (var record in data)
                {
                    var id = options.GetId(record);

                    if (!seenIds.Contains(id))
                    {
                        seenIds.Add(id);

                        hasNewData = true;

                        yield return options.MapData(record);
                    }
                }

                if (hasNewData)
                {
                    queriesWithoutNewData = 0;
                }
                else
                {
                    queriesWithoutNewData++;
                }

                await Task.Delay(s_sleepDurationProvider.Invoke(queriesWithoutNewData), cancellationToken);
            }
        }
    }
}
