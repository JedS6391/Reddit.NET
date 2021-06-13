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
        internal UserDetails(Thing<User> thing)
        {
            Name = thing.Data.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetails" /> class.
        /// </summary>
        /// <param name="user">The user data.</param>
        internal UserDetails(User.Details user)
        {
            Name = user.Name;
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />

        public override string ToString() => $"User [Name = {Name}]";
    }
}