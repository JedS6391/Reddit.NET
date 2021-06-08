using System.Text.Json.Serialization;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

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
    }
}