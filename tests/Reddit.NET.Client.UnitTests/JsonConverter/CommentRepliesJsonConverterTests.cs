using System;
using System.Text;
using System.Text.Json;
using NUnit.Framework;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Internal.Json;

namespace Reddit.NET.Client.UnitTests.JsonConverter
{
    public class CommentRepliesJsonConverterTests
    {
        private JsonSerializerOptions _options;
        private CommentRepliesJsonConverter _converter;

        [SetUp]
        public void Setup()
        {
            _options = new JsonSerializerOptions();
            _converter = new CommentRepliesJsonConverter();

            _options.Converters.Add(new ThingJsonConverterFactory());
        }

        [Test]
        public void Read_ListingObject_ReturnsCorrespondingListingObject()
        {
            const string RawValue = @"
{
    ""kind"": ""Listing"",
    ""data"": {
        ""children"": [
            {
                ""kind"": ""t1"",
                ""data"": {
                    ""parent_id"": ""t1_h3uuald""
                }
            }
        ]
    }
}";

            var valueBytes = Encoding.UTF8.GetBytes(RawValue).AsSpan();

            var reader = new Utf8JsonReader(valueBytes);

            // Advance the reader so its on the first token
            reader.Read();

            var convertedValue = _converter.Read(ref reader, typeof(Listing<IHasParent>), _options);

            Assert.IsNotNull(convertedValue);
            Assert.AreEqual("Listing", convertedValue.Kind);
            Assert.IsNotNull(convertedValue.Data);
            Assert.IsNotNull(convertedValue.Data.Children);
            Assert.IsNotEmpty(convertedValue.Data.Children);
            Assert.AreEqual(1, convertedValue.Data.Children.Count);
            Assert.AreEqual("t1", convertedValue.Data.Children[0].Kind);
            Assert.IsNotNull(convertedValue.Data.Children[0].Data);
            Assert.AreEqual("t1_h3uuald", convertedValue.Data.Children[0].Data.ParentFullName);
        }

        [Test]
        public void Read_EmptyString_ReturnsNull()
        {
            const string RawValue = @"""""";

            var valueBytes = Encoding.UTF8.GetBytes(RawValue).AsSpan();

            var reader = new Utf8JsonReader(valueBytes);

            // Advance the reader so its on the first token
            reader.Read();

            var convertedValue = _converter.Read(ref reader, typeof(Listing<IHasParent>), _options);

            Assert.IsNull(convertedValue);
        }

        [Test]
        public void Read_UnexpectedJsonToken_ThrowsJsonException()
        {
            const string RawValue = @"[]";

            Assert.Throws<JsonException>(() =>
            {
                var valueBytes = Encoding.UTF8.GetBytes(RawValue).AsSpan();

                var reader = new Utf8JsonReader(valueBytes);

                // Advance the reader so its on the first token
                reader.Read();

                _converter.Read(ref reader, typeof(Listing<IHasParent>), _options);
            });
        }
    }
}
