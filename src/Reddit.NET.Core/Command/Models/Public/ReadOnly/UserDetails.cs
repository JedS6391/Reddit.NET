using System;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    /// <summary>
    /// Defines a read-only view of a user.
    /// </summary>
    public class UserDetails
    {   
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetails" /> class.
        /// </summary>
        /// <param name="thing">A <see cref="Thing{TData}" /> containing a user's data.</param>
        internal UserDetails(IThing<User> thing)
        {
            Name = thing.Data.Data.Name;
            CreatedAtUtc = thing.Data.Data.CreatedAtUtc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetails" /> class.
        /// </summary>
        /// <param name="user">The user data.</param>
        internal UserDetails(User.Details user)
        {
            Name = user.Name;
            CreatedAtUtc = user.CreatedAtUtc;
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the date and time the user was created.
        /// </summary>
        public DateTimeOffset CreatedAtUtc { get; }

        /// <inheritdoc />

        public override string ToString() => $"User [Name = {Name}]";
    }
}