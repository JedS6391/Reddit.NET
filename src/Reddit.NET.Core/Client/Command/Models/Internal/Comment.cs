using System.Text.Json.Serialization;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Internal
{
    public class Comment : Thing<Comment.Details>
    {
        public class Details 
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("body")]
            public string Body { get; set; }

            [JsonPropertyName("replies")]
            public object Replies { get; set; }
        }
        
        public class Listing : Listing<Comment.Details> 
        {
        }        
    }
}