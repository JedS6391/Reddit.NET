using System;

namespace Reddit.NET.Core.Client.Command.Exceptions
{
    public class CommandNotSupportedException : Exception
    {
        public CommandNotSupportedException(string message)
            : base(message)
        {
        }   
    }
}