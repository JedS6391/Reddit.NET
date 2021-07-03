using System;

namespace Reddit.NET.Client.Exceptions
{
    /// <summary>
    /// Defines the base object for all exceptions thrown by <see cref="RedditClient" />
    /// </summary>
    public abstract class RedditClientException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClientException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected RedditClientException(string message)
            : base(message)
        {
        }
    }
}