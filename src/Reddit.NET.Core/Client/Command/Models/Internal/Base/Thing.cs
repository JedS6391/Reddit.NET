using System.Text.Json.Serialization;

namespace Reddit.NET.Core.Client.Command.Models.Internal.Base
{
	public class Thing<TData>
	{
		[JsonPropertyName("id")]
		public string Id { get; set; }

		[JsonPropertyName("name")]
        public string Name { get; set; } 

		[JsonPropertyName("kind")]   
        public string Kind { get; set; }    

		[JsonPropertyName("data")]
        public TData Data { get; set; }
	}
}