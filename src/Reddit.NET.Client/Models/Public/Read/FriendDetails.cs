using Microsoft;
using Reddit.NET.Client.Interactions;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a user's friend.
    /// </summary>
    public class FriendDetails : IToInteractor<UserInteractor>
    {
        internal FriendDetails(IThing<Friend.Details> thing)
        {
            Requires.NotNull(thing, nameof(thing));

            Id = thing.Id;
            Name = thing.Name;
        }

        /// <summary>
        /// Gets the identifier of the friend.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the username of the friend.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public UserInteractor Interact(RedditClient client) => client.User(Name);
    }
}
