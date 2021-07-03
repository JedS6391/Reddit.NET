namespace Reddit.NET.Client.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when requests are rate limited.
    /// </summary>    
    public class RateLimitException : RedditClientException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>        
        public RateLimitException(string message) 
            : base(message)
        {
        }
    }
}