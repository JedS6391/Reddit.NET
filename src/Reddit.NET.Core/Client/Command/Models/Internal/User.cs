using System.Text.Json.Serialization;

namespace Reddit.NET.Core.Client.Command.Models.Internal
{
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}