using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    /// <summary>
    /// Defines a read-only view of a subreddit.
    /// </summary>
    public class SubredditDetails : IToInteractor<SubredditInteractor>
    {
        public string Name { get; internal set; }
        public string Title { get; internal set; }

        public SubredditInteractor Interact(RedditClient client) => client.Subreddit(Name);
    }
}