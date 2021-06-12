using System;

namespace Reddit.NET.Core.Client.Builder.Exceptions
{
    /// <summary>
    /// Represents errors that occur during creation of a <see cref="RedditClient" />.
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