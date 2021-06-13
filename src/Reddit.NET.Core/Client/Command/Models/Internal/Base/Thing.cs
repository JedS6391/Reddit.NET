using System.Text.Json.Serialization;

namespace Reddit.NET.Core.Client.Command.Models.Internal.Base
{
	/// <summary>
    /// Defines the base attributes shared by all reddit API objects.
    /// </summary>
    /// <remarks>
    /// Each <see cref="Thing{TData}" /> has a kind which will define the type of data the thing represents 
    /// (e.g. a comment 'thing' will have different data than a subreddit 'thing').
    /// </remarks>
    /// <see href="https://github.com/reddit-archive/reddit/wiki/JSON#thing-reddit-base-class" />
    /// <typeparam name="TData">The type of data this kind of thing contains.</typeparam>
	public class Thing<TData>
	{
		/// <summary>
        /// Gets the identifier of the thing.
        /// </summary>
		[JsonPropertyName("id")]
		[JsonInclude]
		public string Id { get; private set; }

		/// <summary>
        /// Gets the full name of the thing.
        /// </summary>
        /// <remarks>
        /// Full names are a combination of a thing's type and its identifier. For example, <c>t1_h0zsb4o</c>.
        /// </remarks>
		[JsonPropertyName("name")]
		[JsonInclude]
        public string Name { get; private set; } 

		/// <summary>
        /// Gets the kind of the thing.
        /// </summary>
		[JsonPropertyName("kind")]   
		[JsonInclude]
        public string Kind { get; private set; }    

		/// <summary>
        /// Gets the data of the thing.
        /// </summary>
		[JsonPropertyName("data")]
		[JsonInclude]
        public TData Data { get; private set; }
	}
}