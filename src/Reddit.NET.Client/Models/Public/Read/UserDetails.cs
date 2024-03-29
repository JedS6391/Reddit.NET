using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Interactions;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a user.
    /// </summary>
    public class UserDetails : IToInteractor<UserInteractor>, IReloadable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetails" /> class.
        /// </summary>
        /// <param name="thing">A <see cref="Thing{TData}" /> containing a user's data.</param>
        internal UserDetails(IThing<User.Details> thing)
        {
            Requires.NotNull(thing, nameof(thing));

            Name = thing.Data.Name;
            CommentKarma = thing.Data.CommentKarma;
            SubmissionKarma = thing.Data.LinkKarma;
            IsFriend = thing.Data.IsFriend;
            CreatedAtUtc = thing.Data.CreatedAtUtc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetails" /> class.
        /// </summary>
        /// <param name="user">The user data.</param>
        internal UserDetails(User.Details user)
        {
            Requires.NotNull(user, nameof(user));

            Name = user.Name;
            CommentKarma = user.CommentKarma;
            SubmissionKarma = user.LinkKarma;
            IsFriend = user.IsFriend;
            CreatedAtUtc = user.CreatedAtUtc;
            LastLoadedAtUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the karma of the user earned from comments.
        /// </summary>
        public int CommentKarma { get; private set; }

        /// <summary>
        /// Gets the link karma of the user earned from submissions.
        /// </summary>
        public int SubmissionKarma { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user is a friend of the authenticated user.
        /// </summary>
        public bool IsFriend { get; private set; }

        /// <summary>
        /// Gets the date and time the user was created.
        /// </summary>
        public DateTimeOffset CreatedAtUtc { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset LastLoadedAtUtc { get; private set; }

        /// <inheritdoc />
        public UserInteractor Interact(RedditClient client) => client.User(Name);

        /// <inheritdoc />
        public async Task ReloadAsync(RedditClient client, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(client, nameof(client));

            var details = await Interact(client).GetDetailsAsync(cancellationToken);

            Name = details.Name;
            CommentKarma = details.CommentKarma;
            SubmissionKarma = details.SubmissionKarma;
            IsFriend = details.IsFriend;
            CreatedAtUtc = details.CreatedAtUtc;
            LastLoadedAtUtc = DateTimeOffset.UtcNow;
        }

        /// <inheritdoc />
        public override string ToString() => $"User [Name = {Name}]";
    }
}
