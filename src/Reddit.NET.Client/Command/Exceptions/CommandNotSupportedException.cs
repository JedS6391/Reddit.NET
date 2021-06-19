using System;

namespace Reddit.NET.Client.Command.Exceptions
{
    /// <summary>
    /// Represents the error that occurs when a command cannot be executed.
    /// </summary>
    public class CommandNotSupportedException : Exception
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