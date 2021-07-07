using System.Text.Json;
using NUnit.Framework;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Internal.Json;

namespace Reddit.NET.Client.UnitTests.JsonConverter
{
    public class ThingJsonConverterFactoryTests
    {
        private ThingJsonConverterFactory _converterFactory;

        [SetUp]
        public void Setup()
        {
            _converterFactory = new ThingJsonConverterFactory();
        }

        [Test]
        public void CanConvert_IThingTypeWithConcreteGeneric_ReturnsTrue()
        {
            var typeToConvert = typeof(IThing<Subreddit.Details>);

            Assert.IsTrue(_converterFactory.CanConvert(typeToConvert));
        }

        [Test]
        public void CanConvert_IThingTypeWithAbstractGeneric_ReturnsTrue()
        {
            var typeToConvert = typeof(IThing<IHasParent>);

            Assert.IsTrue(_converterFactory.CanConvert(typeToConvert));
        }

        [Test]
        public void CanConvert_NotIThingType_ReturnsFalse()
        {
            var typeToConvert = typeof(object);

            Assert.IsFalse(_converterFactory.CanConvert(typeToConvert));
        }

        [Test]
        public void CreateConverter_IThingTypeWithConcreteGeneric_ReturnsConcreteTypeThingJsonConverter()
        {
            var typeToConvert = typeof(IThing<Subreddit.Details>);

            var converter = _converterFactory.CreateConverter(typeToConvert, new JsonSerializerOptions());

            Assert.IsNotNull(converter);
            Assert.IsInstanceOf<ThingJsonConverterFactory.ConcreteTypeThingJsonConverter<Subreddit.Details, Subreddit>>(converter);
        }

        [Test]
        public void CreateConverter_IThingTypeWithAbstractGeneric_ReturnsDynamicTypeThingJsonConverter()
        {
            var typeToConvert = typeof(IThing<IHasParent>);

            var converter = _converterFactory.CreateConverter(typeToConvert, new JsonSerializerOptions());

            Assert.IsNotNull(converter);
            Assert.IsInstanceOf<ThingJsonConverterFactory.DynamicTypeThingJsonConverter<IHasParent>>(converter);
        }
    }
}
