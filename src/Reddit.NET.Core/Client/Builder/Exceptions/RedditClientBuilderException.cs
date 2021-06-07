using System;

namespace Reddit.NET.Core.Client.Builder.Exceptions
{
    public class RedditClientBuilderException : Exception
    {
        public RedditClientBuilderException(string message)
            : base(message)            
        {
        }
    }
}