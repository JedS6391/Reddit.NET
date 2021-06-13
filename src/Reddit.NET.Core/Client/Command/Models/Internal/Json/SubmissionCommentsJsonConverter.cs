using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reddit.NET.Core.Client.Command.Models.Internal.Json
{
    /// <summary>
    /// A <see cref="JsonConverter{T}" /> implementation for <see cref="Submission.SubmissionComments" /> instances.
    /// </summary>
    /// <remarks>
    /// A <c>GET /r/{subreddit}/comments/{article}</c> request returns an array with two elements:
    ///   
    ///   1. Listing of submission things (with a single child for the submission in question)
    ///   2. Listing of comment things
    ///   
    /// This converter will read both parts from the array and encapsulate them in a <see cref="Submission.SubmissionComments" /> instance.
    /// </remarks>
    internal class SubmissionCommentsJsonConverter : JsonConverter<Submission.SubmissionComments>
    {
        /// <inheritdoc />
        public override Submission.SubmissionComments Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // [
            //     {
            //         * submission listing *
            //     },
            //     {
            //         * comment listing *
            //     }
            // ]

            // Begin array 
            reader.Consume(JsonTokenType.StartArray);

            // Start first object
            reader.Match(JsonTokenType.StartObject);

            // Read submission listing
            var submissions = JsonSerializer.Deserialize<Submission.Listing>(ref reader);

            // End first object            
            reader.Consume(JsonTokenType.EndObject);

            // Start second object
            reader.Match(JsonTokenType.StartObject);

            // Read comment listing
            var comments = JsonSerializer.Deserialize<Comment.Listing>(ref reader);

            // End second object
            reader.Consume(JsonTokenType.EndObject);

            // End array
            reader.Consume(JsonTokenType.EndArray);

            return new Submission.SubmissionComments()
            {
                Submissions = submissions,
                Comments = comments
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Submission.SubmissionComments value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}