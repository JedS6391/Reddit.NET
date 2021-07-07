namespace Reddit.NET.Client.Command
{
    /// <summary>
    /// Defines the URLs for the reddit APIs used by the client.
    /// </summary>
    internal static class RedditApiUrl
    {
        private const string RedditBasePath = "https://www.reddit.com";
        private const string RedditOAuthPath = "https://oauth.reddit.com";

        public static class Authentication
        {
            public static string Token => $"{RedditBasePath}/api/v1/access_token";
        }

        public static class Me
        {
            public static string Details => $"{RedditOAuthPath}/api/v1/me";
            public static string Subreddits => $"{RedditOAuthPath}/subreddits/mine/subscriber";
            public static string KarmaBreakdown => $"{RedditOAuthPath}/api/v1/me/karma";
            public static string Trophies => $"{RedditOAuthPath}/api/v1/me/trophies";
            public static string Multireddits => $"{RedditOAuthPath}/api/multi/mine";
            public static string CreateMultireddit => $"{RedditOAuthPath}/api/multi";
            public static string Inbox(string messageType) => $"{RedditOAuthPath}/message/{messageType}";
            public static string SendMessage => $"{RedditOAuthPath}/api/compose";
        }

        public static class User
        {
            public static string Details(string username) => $"{RedditOAuthPath}/user/{username}/about";
            public static string History(string username, string type) => $"{RedditOAuthPath}/user/{username}/{type}";
            public static string Trophies(string username) => $"{RedditOAuthPath}/api/v1/user/{username}/trophies";
        }

        public static class Subreddit
        {
            public static string Details(string subredditName) => $"{RedditOAuthPath}/r/{subredditName}/about";
            public static string Submissions(string subredditName, string sort)
                => $"{RedditOAuthPath}/r/{subredditName}/{sort}";
            public static string FrontPageSubmissions(string sort) => $"{RedditOAuthPath}/{sort}";
            public static string Search(string subredditName)
                => $"{RedditOAuthPath}/r/{subredditName}/search";
            public static string Subscription => $"{RedditOAuthPath}/api/subscribe";
            public static string Submit => $"{RedditOAuthPath}/api/submit";
        }

        public static class Multireddit
        {
            public static string Details(string username, string multiredditName) =>
                $"{RedditOAuthPath}/api/multi/user/{username}/m/{multiredditName}";

            public static string UpdateSubreddit(string username, string multiredditName, string subredditName) =>
                $"{RedditOAuthPath}/api/multi/user/{username}/m/{multiredditName}/r/{subredditName}";
        }

        public static class Submission
        {
            public static string DetailsWithComments(string submissionId) =>
                $"{RedditOAuthPath}/comments/{submissionId}";
        }

        public static class UserContent
        {
            public static string Vote => $"{RedditOAuthPath}/api/vote/";
            public static string Save => $"{RedditOAuthPath}/api/save";
            public static string Unsave => $"{RedditOAuthPath}/api/unsave";
            public static string Reply => $"{RedditOAuthPath}/api/comment";
            public static string Delete => $"{RedditOAuthPath}/api/del";
        }
    }
}
