using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.Models.Public.Streams;

namespace Reddit.NET.Client.UnitTests
{
    public class PollingStreamTests
    {
        [Test]
        public async Task PollingStream_PeriodicDataSource_ReturnsDataAsItBecomesAvailable()
        {
            var dataSource = new UpdateableDataSource();

            var stream = PollingStream.Create(new PollingStreamOptions<string, string, string>(
                () => dataSource.GetData(),
                mapper: v => v,
                idSelector: v => v));

            var data = await stream.Take(50).ToListAsync();

            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);
        }

        private class UpdateableDataSource
        {
            private readonly Random _random;
            private readonly Timer _timer;
            private IEnumerable<string> _data;

            public UpdateableDataSource()
            {
                _random = new Random();
                _timer = new Timer(
                    (_) => GenerateData(),
                    this,
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(3));
            }

            public Task<IEnumerable<string>> GetData() => Task.FromResult(_data);

            private void GenerateData()
            {
                _data = Enumerable
                    .Range(0, _random.Next(1, 5))
                    .Select(v => Guid.NewGuid().ToString());
            }
        }
    }
}
