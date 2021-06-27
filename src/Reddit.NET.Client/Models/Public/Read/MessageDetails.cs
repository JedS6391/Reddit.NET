using System;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a comment.
    /// </summary>
    public class MessageDetails
    {
        internal MessageDetails(IThing<Message.Details> thing)            
        {
            Author = thing.Data.Author;
            Body = thing.Data.Body;
            CreatedAtUtc = thing.Data.CreatedAtUtc;
        }

        /// <summary>
        /// Gets the author of the message.
        /// </summary>
        public string Author { get; protected set; }

        /// <summary>
        /// Gets the body of the message.
        /// </summary>
        public string Body { get; }        

        /// <summary>
        /// Gets the date and time the message was created.
        /// </summary>
        public DateTimeOffset CreatedAtUtc { get; protected set; }    

        /// <inheritdoc />
        public override string ToString() => 
            $"Message [Author = {Author}, Body = {Body}, CreatedAtUtc = {CreatedAtUtc}]";            
    }
}