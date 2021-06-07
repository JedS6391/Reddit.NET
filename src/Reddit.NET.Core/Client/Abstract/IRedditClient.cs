using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client.Abstract
{
    public interface IRedditClient
    {
        SubredditInteractor Subreddit(string displayName);
    }
}