using System.Text.Json.Serialization;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Internal.Json;

namespace Reddit.NET.Core.Client.Command.Models.Internal
{
    public class Submission : Thing<Submission.Details>
    {
        public class Details 
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("subreddit")]
            public string Subreddit { get; set; }

            [JsonPropertyName("permalink")]
            public string Permalink { get; set; }
        }

        public class Listing : Listing<Submission.Details> 
        {
        }

        [JsonConverter(typeof(SubmissionCommentsJsonConverter))]
        internal class SubmissionComments
        {
            public Submission.Listing Submissions { get; set;}
            public Comment.Listing Comments { get; set;}
        }
    }
}