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
            Assert.AreEqual(2021, convertedValue.Date.Year);
            Assert.AreEqual(6, convertedValue.Date.Month);
            Assert.AreEqual(13, convertedValue.Date.Day);
            Assert.AreEqual(DayOfWeek.Sunday, convertedValue.Date.DayOfWeek);
            Assert.AreEqual(17, convertedValue.Date.Hour);
            Assert.AreEqual(35, convertedValue.Date.Minute);
            Assert.AreEqual(29, convertedValue.Date.Second);
            Assert.AreEqual(0, convertedValue.Date.Millisecond);            
            Assert.AreEqual(TimeSpan.FromSeconds(0), convertedValue.Date.Offset);
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
            Assert.AreEqual(2021, convertedValue.Year);
            Assert.AreEqual(6, convertedValue.Month);
            Assert.AreEqual(28, convertedValue.Day);
            Assert.AreEqual(DayOfWeek.Monday, convertedValue.DayOfWeek);
            Assert.AreEqual(7, convertedValue.Hour);
            Assert.AreEqual(6, convertedValue.Minute);
            Assert.AreEqual(35, convertedValue.Second);
            Assert.AreEqual(0, convertedValue.Millisecond);            
            Assert.AreEqual(TimeSpan.FromSeconds(0), convertedValue.Offset);
        }

        [Test]
        public void Read_InalidObject_ThrowsJsonException()
        {
            const string rawValue = "{ \"date\": \"test\" }";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestObject>(rawValue));
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
            // 1 less than the range expected when converting Unix seconds to DateTimeOffset:
            // https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.fromunixtimeseconds?view=net-5.0#exceptions
            const string rawValue = "-62135596801.0";

            // Note we have all the set up inside the anonymous method as ref parameters
            // cannot be used inside anonymous methods.
            var exception = Assert.Throws<JsonException>(() =>
            {
                var rawValueBytes = Encoding.UTF8.GetBytes(rawValue).AsSpan();

                var reader = new Utf8JsonReader(rawValueBytes);

                // Advance the reader so its on the first token
                reader.Read();

                _converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
            });

            Assert.IsNotNull(exception.InnerException);
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(exception.InnerException);            
        } 

        [Test]
        public void Read_OutsideOfRangePositiveNumericValue_ThrowsJsonException()
        {
            // 1 greater than the range expected when converting Unix seconds to DateTimeOffset:
            // https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset.fromunixtimeseconds?view=net-5.0#exceptions            
            const string rawValue = "253402300800.0";

            // Note we have all the set up inside the anonymous method as ref parameters
            // cannot be used inside anonymous methods.
            var exception = Assert.Throws<JsonException>(() =>
            {
                var rawValueBytes = Encoding.UTF8.GetBytes(rawValue).AsSpan();

                var reader = new Utf8JsonReader(rawValueBytes);

                // Advance the reader so its on the first token
                reader.Read();

                _converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
            });

            Assert.IsNotNull(exception.InnerException);
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(exception.InnerException);
        }       

        private class TestObject
        {
            [JsonPropertyName("date")]
            [JsonConverter(typeof(EpochSecondJsonConverter))]
            public DateTimeOffset Date { get; set; }
        }           
    }
}