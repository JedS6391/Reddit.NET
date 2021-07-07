using System;

namespace Reddit.NET.Client.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when building a <see cref="RedditClient" /> instance.
    /// </summary>
    public class RedditClientBuilderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClientBuilderException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RedditClientBuilderException(string message)
            : base(message)
        {
        }
    }
}
