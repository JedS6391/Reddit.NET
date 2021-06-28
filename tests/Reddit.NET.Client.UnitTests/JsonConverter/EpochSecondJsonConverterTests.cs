using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using NUnit.Framework;
using Reddit.NET.Client.Models.Internal.Json;

namespace Reddit.NET.Client.UnitTests.JsonConverter
{
    public class EpochSecondJsonConverterTests
    {
        private EpochSecondJsonConverter _converter;

        [SetUp]
        public void Setup()
        {
            _converter = new EpochSecondJsonConverter();
        }

        [Test]
        public void Read_ValidObject_ReturnsCorrespondingObjectWithConvertedDateTimeOffset()
        {
            const string rawValue = "{ \"date\": 1623605729.0 }";

            var convertedValue = JsonSerializer.Deserialize<TestObject>(rawValue);

            // The value above should correspond to Sun Jun 13 2021 17:35:29 GMT+0000
            Assert.IsNotNull(convertedValue);
            Assert.AreEqual(convertedValue.Date.Year, 2021);
            Assert.AreEqual(convertedValue.Date.Month, 6);
            Assert.AreEqual(convertedValue.Date.Day, 13);
            Assert.AreEqual(convertedValue.Date.DayOfWeek, DayOfWeek.Sunday);
            Assert.AreEqual(convertedValue.Date.Hour, 17);
            Assert.AreEqual(convertedValue.Date.Minute, 35);
            Assert.AreEqual(convertedValue.Date.Second, 29);
            Assert.AreEqual(convertedValue.Date.Millisecond, 0);            
            Assert.AreEqual(convertedValue.Date.Offset, TimeSpan.FromSeconds(0));
        }

        [Test]
        public void Read_ValidDoubleValue_ReturnsCorrespondingDateTimeOffset()
        {
            const string rawValue = "1624863995.0";

            var valueBytes = Encoding.UTF8.GetBytes(rawValue).AsSpan();

            var reader = new Utf8JsonReader(valueBytes);

            // Advance the reader so its on the first token
            reader.Read();

            var convertedValue = _converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());

            // The value above should correspond to Mon Jun 28 2021 07:06:35 GMT+0000
            Assert.AreEqual(convertedValue.Year, 2021);
            Assert.AreEqual(convertedValue.Month, 6);
            Assert.AreEqual(convertedValue.Day, 28);
            Assert.AreEqual(convertedValue.DayOfWeek, DayOfWeek.Monday);
            Assert.AreEqual(convertedValue.Hour, 7);
            Assert.AreEqual(convertedValue.Minute, 6);
            Assert.AreEqual(convertedValue.Second, 35);
            Assert.AreEqual(convertedValue.Millisecond, 0);            
            Assert.AreEqual(convertedValue.Offset, TimeSpan.FromSeconds(0));
        }

        [Test]
        public void Read_InalidObject_ThrowsJsonException()
        {
            const string rawValue = "{ \"date\": \"test\" }";

            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<TestObject>(rawValue));
        }        

        [Test]
        public void Read_NonNumericValue_ThrowsJsonException()
        {
            const string rawValue = "\"test\"";

            // Note we have all the set up inside the anonymous method as ref parameters
            // cannot be used inside anonymous methods.
            Assert.Throws<JsonException>(() =>
            {
                var rawValueBytes = Encoding.UTF8.GetBytes(rawValue).AsSpan();

                var reader = new Utf8JsonReader(rawValueBytes);

                // Advance the reader so its on the first token
                reader.Read();

                _converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
            });
        }   

        [Test]
        public void Read_OutsideOfRangeNegativeNumericValue_ThrowsJsonException()
        {
            const string rawValue = "-62135596801.0";

            // Note we have all the set up inside the anonymous method as ref parameters
            // cannot be used inside anonymous methods.
            Assert.Throws<JsonException>(() =>
            {
                var rawValueBytes = Encoding.UTF8.GetBytes(rawValue).AsSpan();

                var reader = new Utf8JsonReader(rawValueBytes);

                // Advance the reader so its on the first token
                reader.Read();

                _converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
            });
        } 

        [Test]
        public void Read_OutsideOfRangePositiveNumericValue_ThrowsJsonException()
        {
            const string rawValue = "253402300800.0";

            // Note we have all the set up inside the anonymous method as ref parameters
            // cannot be used inside anonymous methods.
            Assert.Throws<JsonException>(() =>
            {
                var rawValueBytes = Encoding.UTF8.GetBytes(rawValue).AsSpan();

                var reader = new Utf8JsonReader(rawValueBytes);

                // Advance the reader so its on the first token
                reader.Read();

                _converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
            });
        }

        private class TestObject
        {
            [JsonPropertyName("date")]
            [JsonConverter(typeof(EpochSecondJsonConverter))]
            public DateTimeOffset Date { get; set; }
        }           
    }
}