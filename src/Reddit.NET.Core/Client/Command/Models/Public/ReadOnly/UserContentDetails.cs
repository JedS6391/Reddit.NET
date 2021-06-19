using System;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    /// <summary>
    /// Defines a read-only view of user content.
    /// </summary>
    public abstract class UserContentDetails
    {
        /// <summary>
        /// Gets the author of the content.
        /// </summary>
        public string Author { get; protected set; }       

        /// <summary>
        /// Gets the number of upvotes on the content.
        /// </summary>
        public int Upvotes { get; protected set; }

        /// <summary>
        /// Gets the number of downvotes on the content.
        /// </summary>
        public int Downvotes { get; protected set; }     

        /// <summary>
        /// Gets the date and time the content was created.
        /// </summary>
        public DateTimeOffset CreatedAtUtc { get; protected set; }
    }
}