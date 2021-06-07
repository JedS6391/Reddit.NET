using System;

namespace Reddit.NET.Core.Client.Command.Exceptions
{
    public class CommandExecutionFailedException : Exception
    {
        public CommandExecutionFailedException(string message) 
            : base(message)
        {}
    }
}