using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reddit.NET.Client.Interactions;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a multireddit.
    /// </summary>
    public class MultiredditDetails : IToInteractor<MultiredditInteractor>, IReloadable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiredditDetails" /> class.
        /// </summary>
        /// <param name="thing">A <see cref="Thing{TData}" /> containing a multireddit's data.</param>
        internal MultiredditDetails(IThing<Multireddit.Details> thing)
        {
            Name = thing.Data.DisplayName;
            Subreddits = thing.Data.Subreddits.Select(s => s.Name).ToList();
            Username = thing.Data.OwnerUsername;
            LastLoadedAtUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Gets the name of the multireddit.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the names of the subreddits that the multireddit is comprised of.
        /// </summary>
        public IReadOnlyList<string> Subreddits { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset LastLoadedAtUtc { get; private set; }

        /// <summary>
        /// Gets the name of the user the multireddit belongs to.
        /// </summary>
        internal string Username { get; private set; }

        /// <inheritdoc />
        public MultiredditInteractor Interact(RedditClient client) => client.Multireddit(Username, Name);

        /// <inheritdoc />
        public async Task ReloadAsync(RedditClient client)
        {
            var details = await Interact(client).GetDetailsAsync();

            Name = details.Name;
            Subreddits = details.Subreddits;
            LastLoadedAtUtc = DateTimeOffset.UtcNow;
        }

        /// <inheritdoc />
        public override string ToString() => $"Multireddit [Name = {Name}]";
    }
}
