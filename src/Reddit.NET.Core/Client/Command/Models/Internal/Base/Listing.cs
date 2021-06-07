using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reddit.NET.Core.Client.Command.Models.Internal.Base
{
    public class Listing<TData>
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("data")]
        public ListingData<TData> Data { get; set; }
    }

    public class ListingData<TData>
    {
        [JsonPropertyName("before")]
        public string Before { get; set; }

        [JsonPropertyName("after")]
        public string After { get; set; }

        [JsonPropertyName("children")]
        public List<Thing<TData>> Children { get; set; }
    }
}