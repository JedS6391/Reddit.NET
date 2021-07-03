using Reddit.NET.Client.Exceptions;

namespace Reddit.NET.Client.Exceptions
{
    /// <summary>
    /// Represents the error that occurs when a command cannot be executed.
    /// </summary>
    public sealed class CommandNotSupportedException : RedditClientException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotSupportedException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public CommandNotSupportedException(string message)
            : base(message)
        {
        }   
    }
}