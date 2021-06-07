using System;

namespace Reddit.NET.Core.Client.Command.Exceptions
{
    public class CommandCreationFailureException : Exception
    {
        
        public CommandCreationFailureException(string message)
            : base(message)
        {
        }
    }
}