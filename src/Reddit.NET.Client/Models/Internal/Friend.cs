using Reddit.NET.Client.Models.Internal.Base;

namespace Reddit.NET.Client.Models.Internal
{
    /// <summary>
    /// Represents a friend of a reddit user.
    /// </summary>
    public class Friend : Thing<Friend.Details>
    {
        /// <summary>
        /// Defines the attributes of a <see cref="Friend" />.
        /// </summary>
        public class Details
        {

        }

        /// <summary>
        /// Defines a listing over a collection of <see cref="Friend" /> things.
        /// </summary>
        public class Listing : Listing<Details>
        {

        }
    }
}
