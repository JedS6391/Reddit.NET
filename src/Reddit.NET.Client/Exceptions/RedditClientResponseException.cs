using System.Net;

namespace Reddit.NET.Client.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when the reddit API returns a non-successful status code.
    /// </summary>
    /// <remarks>
    /// This exception occurs when the response has a status code that the client does not have
    /// a specific exception type for.
    /// </remarks>
    public sealed class RedditClientResponseException : RedditClientException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClientResponseException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// /// <param name="statusCode">The status code of the response that caused the error.</param>
        public RedditClientResponseException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Gets the status code of the response that caused the error.
        /// </summary>
        public HttpStatusCode StatusCode { get; }
    }
}
