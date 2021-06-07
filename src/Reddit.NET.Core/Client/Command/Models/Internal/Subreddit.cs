using System.Text.Json.Serialization;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Internal
{
    public class Subreddit : Thing<Subreddit.Details>
    {
        public class Details
        {            
            [JsonPropertyName("id")]
            public string Id { get; set; }         
        
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("display_name")]
            public string DisplayName { get; set; }
        }
    }
}