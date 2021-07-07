using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Internal.Json
{
    /// <summary>
    /// A <see cref="JsonConverter{T}" /> implementation for <see cref="Submission.SubmissionWithComments" /> instances.
    /// </summary>
    /// <remarks>
    /// A <c>GET /r/{subreddit}/comments/{article}</c> request returns an array with two elements:
    ///
    ///   1. Listing of submission things (with a single child for the submission in question)
    ///   2. Listing of comment things
    ///
    /// This converter will read both parts from the array and encapsulate them in a <see cref="Submission.SubmissionWithComments" /> instance.
    /// </remarks>
    internal class SubmissionWithCommentsJsonConverter : JsonConverter<Submission.SubmissionWithComments>
    {
        /// <inheritdoc />
        public override Submission.SubmissionWithComments Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // [
            //     {
            //         * submission listing *
            //     },
            //     {
            //         * comment listing *
            //     }
            // ]

            // Begin array.
            reader.Consume(JsonTokenType.StartArray);

            // Start first object.
            reader.Match(JsonTokenType.StartObject);

            // Read submission listing.
            var submissions = JsonSerializer.Deserialize<Submission.Listing>(ref reader, options);

            // End first object.
            reader.Consume(JsonTokenType.EndObject);

            // Start second object.
            reader.Match(JsonTokenType.StartObject);

            // Read comment listing.
            // Note we treat the comment listing as a very generic IHasParent model, as it will
            // contain both Comment and MoreChildren objects.
            var comments = JsonSerializer.Deserialize<Listing<IHasParent>>(ref reader, options);

            // End second object
            reader.Consume(JsonTokenType.EndObject);

            // End array.
            reader.Consume(JsonTokenType.EndArray);

            return new Submission.SubmissionWithComments(
                submissions,
                comments);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Submission.SubmissionWithComments value, JsonSerializerOptions options) =>
            throw new NotImplementedException();
    }
}
