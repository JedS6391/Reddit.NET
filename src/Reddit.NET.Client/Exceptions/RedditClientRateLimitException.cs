using System.Net;

namespace Reddit.NET.Client.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when requests are rate limited.
    /// </summary>
    /// <remarks>
    /// This exception occurs when too many requests have been made,
    /// resulting in a response with a <see cref="HttpStatusCode.BadRequest" /> status code.
    /// </remarks>
    public class RedditClientRateLimitException : RedditClientException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClientRateLimitException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RedditClientRateLimitException(string message)
            : base(message)
        {
        }
    }
}
