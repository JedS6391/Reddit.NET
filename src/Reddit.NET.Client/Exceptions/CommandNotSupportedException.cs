namespace Reddit.NET.Client.Exceptions
{
    /// <summary>
    /// Represents the error that occurs when a command cannot be executed.
    /// </summary>
    /// <remarks>
    /// This exception occurs when a command is executed in an invalid authentication context
    /// (e.g. a user-based command in a read-only context).
    /// </remarks>
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
