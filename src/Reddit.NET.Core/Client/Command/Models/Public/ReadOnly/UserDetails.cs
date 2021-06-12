using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    public class UserDetails
    {
        internal UserDetails(User user)
        {
            Name = user.Name;
        }

        internal UserDetails(Thing<User> user)
        {
            Name = user.Name;
        }

        public string Name { get; }
    }
}