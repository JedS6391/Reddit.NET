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
        internal SubredditDetails(Thing<Subreddit.Details> subreddit)
        {
            Name = subreddit.Data.DisplayName;
            Title = subreddit.Data.Title;
        }

        public string Name { get; }
        public string Title { get; }

        public SubredditInteractor Interact(RedditClient client) => client.Subreddit(Name);

        public override string ToString() => $"Subreddit [Name = {Name}, Title = {Title}]";
    }
}