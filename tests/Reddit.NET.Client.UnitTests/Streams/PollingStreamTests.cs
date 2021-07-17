using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Reddit.NET.Client.Models.Public.Streams;

namespace Reddit.NET.Client.UnitTests.Streams
{
    public class PollingStreamTests
    {
        [Test]
        public async Task PollingStream_PeriodicDataSource_ReturnsDataAsItBecomesAvailable()
        {
            var dataSource = new PeriodicDataSource();

            var stream = PollingStream.Create(new PollingStreamOptions<string, string, string>(
                (ct) => dataSource.GetData(),
                mapper: v => v,
                idSelector: v => v));

            var data = await stream.Take(30).ToListAsync();

            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);
        }

        private class PeriodicDataSource
        {
            private readonly Random _random;
            private readonly Timer _timer;
            private IEnumerable<string> _data;

            public PeriodicDataSource()
            {
                _random = new Random();
                _timer = new Timer(
                    (_) => GenerateData(),
                    this,
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(5));
            }

            public Task<IEnumerable<string>> GetData() => Task.FromResult(_data);

            private void GenerateData()
            {
                _data = Enumerable
                    .Range(0, _random.Next(1, 10))
                    .Select(v => Guid.NewGuid().ToString())
                    .ToList();
            }
        }
    }
}
