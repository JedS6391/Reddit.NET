using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    /// <summary>
    /// Defines a read-only view of a subreddit.
    /// </summary>
    public class SubredditDetails : IToInteractor<SubredditInteractor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetails" /> class.
        /// </summary>
        /// <param name="thing">A <see cref="Thing{TData}" /> containg a subreddit's data.</param>
        internal SubredditDetails(IThing<Subreddit.Details> thing)
        {
            Name = thing.Data.DisplayName;
            Title = thing.Data.Title;
            Description = thing.Data.Description;
            Subscribers = thing.Data.Subscribers;
        }

        /// <summary>
        /// Gets the name of the subreddit.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the title of the subreddit.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the description of the subreddit.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the number of users subreddit to the subreddit.
        /// </summary>
        public long Subscribers { get; }        

        /// <inheritdoc />
        public SubredditInteractor Interact(RedditClient client) => client.Subreddit(Name);

        /// <inheritdoc />
        public override string ToString() => $"Subreddit [Name = {Name}, Title = {Title}]";
    }
}