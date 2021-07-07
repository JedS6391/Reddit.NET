namespace Reddit.NET.Client.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when an invalid user history type is configured.
    /// </summary>
    public class InvalidUserHistoryTypeException : RedditClientException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUserHistoryTypeException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidUserHistoryTypeException(string message)
            : base(message)
        {
        }
    }
}
