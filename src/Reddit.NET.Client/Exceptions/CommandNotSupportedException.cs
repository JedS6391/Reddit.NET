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
        /// <param name="message">The message that describes the error.</param>
        /// /// <param name="commandId">The identifier of the command that caused the error.</param>
        public CommandNotSupportedException(string message, string commandId)
            : base(message)
        {
            CommandId = commandId;
        }

        /// <summary>
        /// Gets the identifier of the command that caused the error.
        /// </summary>
        public string CommandId { get; }
    }
}
