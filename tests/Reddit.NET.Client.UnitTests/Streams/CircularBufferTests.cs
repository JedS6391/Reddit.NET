using System;
using System.Linq;
using NUnit.Framework;
using Reddit.NET.Client.Models.Public.Streams;

namespace Reddit.NET.Client.UnitTests.Streams
{
    public class CircularBufferTests
    {
        [Test]
        public void Add_CapacityNotMet_ContainsAllItemsAdded()
        {
            var buffer = new CircularBuffer<int>(capacity: 50);

            var values = Enumerable.Range(0, 40).ToList();

            foreach (var v in values)
            {
                buffer.Add(v);
            }

            Assert.IsTrue(values.All(v => buffer.Contains(v)));
        }

        [Test]
        public void Add_CapacityMet_ContainsLatestItemsAdded()
        {
            var buffer = new CircularBuffer<int>(capacity: 50);

            var values = Enumerable.Range(0, 60).ToList();

            foreach (var v in values)
            {
                buffer.Add(v);
            }

            Assert.IsFalse(values.All(v => buffer.Contains(v)));
            Assert.IsFalse(values.Take(10).Any(v => buffer.Contains(v)));
            Assert.IsTrue(values.TakeLast(50).All(v => buffer.Contains(v)));
        }

        [Test]
        public void Contains_ValueInBuffer_ReturnsTrue()
        {
            var buffer = new CircularBuffer<int>(capacity: 1);

            buffer.Add(1);

            Assert.IsTrue(buffer.Contains(1));
        }

        [Test]
        public void Contains_ValueNotInBuffer_ReturnsFalse()
        {
            var buffer = new CircularBuffer<int>(capacity: 1);

            buffer.Add(1);

            Assert.IsFalse(buffer.Contains(2));
        }

        [Test]
        [TestCase(1)]
        [TestCase(30)]
        [TestCase(100)]
        public void Constructor_CapacityGreaterThanZero_CreatesInstances(int capacity)
        {
            var buffer = new CircularBuffer<int>(capacity);

            Assert.IsNotNull(buffer);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-30)]
        [TestCase(-100)]
        public void Constructor_CapacityLessThanOrEqualZero_ThrowsArgumentException(int capacity)
        {
            Assert.Throws<ArgumentException>(() => new CircularBuffer<int>(capacity));
        }
    }
}
