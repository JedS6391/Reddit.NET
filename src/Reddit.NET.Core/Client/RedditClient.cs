using Reddit.NET.Core.Client.Abstract;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client
{
    public class RedditClient : IRedditClient
    {
        private readonly CommandFactory _commandFactory;
        private readonly IAuthenticator _authenticator;        

        internal RedditClient(CommandFactory commandFactory, IAuthenticator authenticator)
        {
            _commandFactory = commandFactory;
            _authenticator = authenticator;
        }

        public MeInteractor Me() => new MeInteractor(_commandFactory, _authenticator);

        public SubredditInteractor Subreddit(string displayName) => 
            new SubredditInteractor(_commandFactory, _authenticator, displayName);
    }
}