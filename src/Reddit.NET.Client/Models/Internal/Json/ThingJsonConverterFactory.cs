using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Internal.Json
{
    /// <summary>
    /// A factory for <see cref="JsonConverter{T}" /> instances responsible for converting <see cref="IThing{TData}" /> objects.
    /// </summary>
    /// <remarks>
    /// Internally, we use the <see cref="IThing{TData}" /> abstraction to represent the data returned by reddit. We can't
    /// convert to an abstract type though, so we need a converter to handle that.
    /// 
    /// It would be possible to instead have a concrete 'thing' class instead of the interface and avoid this, but the 
    /// interface allows us to the handle polymorphic data that certain reddit endpoints return.
    /// </remarks>
    internal class ThingJsonConverterFactory : JsonConverterFactory
    {
        // For certain thing data types, we can take a fast path of direct conversion to the concrete implementation (e.g. IThing<Comment.Details> -> Comment).
        private static readonly IDictionary<Type, Type> s_concreteThingTypes = new Dictionary<Type, Type>()
        {
            { typeof(Comment.Details), typeof(Comment) },
            { typeof(User.Details), typeof(User) },
            { typeof(Submission.Details), typeof(Submission) },
            { typeof(Subreddit.Details), typeof(Subreddit) },
            { typeof(KarmaBreakdown.Details), typeof(KarmaBreakdown) },
            { typeof(Message.Details), typeof(Message) }
        };

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert) =>
            typeToConvert.IsGenericType &&
            typeToConvert.GetGenericTypeDefinition() == typeof(IThing<>);    

        /// <inheritdoc />
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type dataType = typeToConvert.GetGenericArguments().First();

            if (s_concreteThingTypes.TryGetValue(dataType, out Type thingType))
            {
                // We know there is a concrete implementation for this type of thing so use that.
                // This path will be used for a conversion of a type such as IThing<Comment.Details> or IThing<Submission.Details>.
                return (JsonConverter) Activator.CreateInstance(
                    typeof(ConcreteTypeThingJsonConverter<,>).MakeGenericType(
                        new Type[] { dataType, thingType }),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: Array.Empty<object>(),
                    culture: null);
            }

            // There is no concrete implementation for this data type, so we need to dynamically convert each value.
            // This path will be used for a conversion of a type such as IThing<IUserContent> or IThing<IVoteable>.
            // It must be ensured that any values processed can be cast to IThing<TData>.
            return (JsonConverter) Activator.CreateInstance(
                typeof(DynamicTypeThingJsonConverter<>).MakeGenericType(
                    new Type[] { dataType }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: Array.Empty<object>(),
                culture: null);
        }        

        /// <summary>
        /// A <see cref="JsonConverter{T}" /> implementation for converting JSON data to a concrete thing type as specified by <typeparamref name="TThing" />
        /// </summary>
        /// <typeparam name="TData">The type of data the thing contains.</typeparam>
        /// <typeparam name="TThing">The type of the concrete thing implementation.</typeparam>
        internal class ConcreteTypeThingJsonConverter<TData, TThing> : JsonConverter<IThing<TData>>
        {
            /// <inheritdoc />
            public override IThing<TData> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var thing = JsonSerializer.Deserialize<TThing>(ref reader, options);
                
                if (thing is not IThing<TData>)
                {
                    throw new JsonException($"Unable to cast thing with type '{typeof(TThing).FullName}' to '{typeof(IThing<TData>).FullName}'.");
                }

                return thing as IThing<TData>;                
            }
            
            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, IThing<TData> value, JsonSerializerOptions options) => throw new NotImplementedException();            
        }

        /// <summary>
        /// A <see cref="JsonConverter{T}" /> implementation for converting JSON data to a thing type dynamically based on its kind.
        /// </summary>
        /// <typeparam name="TData">The type of data the thing contains.</typeparam>        
        internal class DynamicTypeThingJsonConverter<TData> : JsonConverter<IThing<TData>>
        {
            /// <inheritdoc />
            public override IThing<TData> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // Take a copy of the reader, so it can be deserialized after the initial parse to determine the type.            
                Utf8JsonReader readerCopy = reader;

                // TODO: It would be more inefficient to read the 'kind' property directly via the reader.
                // The down side is it is more complex, so for now we simply parse the entire data.
                if (!JsonDocument.TryParseValue(ref readerCopy, out JsonDocument document))
                {
                    throw new JsonException("Unable to parse JSON document.");
                }

                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new JsonException($"Unexpected JSON value kind during dynamic conversion. Expected '{JsonValueKind.Object}' but was '{document.RootElement.ValueKind}'");
                }

                if (!document.RootElement.TryGetProperty("kind", out JsonElement kindPropertyElement))
                {
                    throw new JsonException("Unable to find 'kind' property in JSON data.");
                }

                var kind = kindPropertyElement.GetString();

                Type type = kind switch
                {
                    Constants.Kind.Comment => typeof(Comment),
                    Constants.Kind.User => typeof(User),
                    Constants.Kind.Submission => typeof(Submission),
                    Constants.Kind.Message => typeof(Message),
                    Constants.Kind.Subreddit => typeof(Subreddit),
                    Constants.Kind.MoreComments => typeof(MoreComments),
                    _ => throw new JsonException($"Unsupported thing kind '{kind}'."),
                };

                object thing = JsonSerializer.Deserialize(ref reader, type, options);

                if (thing is not IThing<TData>)
                {
                    throw new JsonException($"Unable to cast thing with type '{thing.GetType().FullName}' to '{typeof(IThing<TData>).FullName}'.");
                }

                return thing as IThing<TData>;
            }
            
            /// <inheritdoc />
            public override void Write(Utf8JsonWriter writer, IThing<TData> value, JsonSerializerOptions options) => 
                throw new NotImplementedException();           
        }
    }
}