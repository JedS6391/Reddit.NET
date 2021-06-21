namespace Reddit.NET.Client.Command
{
    /// <summary>
    /// Defines the URLs for the reddit APIs used by the client.
    /// </summary>
    internal static class RedditApiUrl
    {
        public static class Authentication
        {
            public static string Token => "https://www.reddit.com/api/v1/access_token";
        }

        public static class Me 
        {
            public static string Details => "https://oauth.reddit.com/api/v1/me";
            public static string Subreddits => "https://oauth.reddit.com/subreddits/mine/subscriber";
            public static string KarmaBreakdown => "https://oauth.reddit.com/api/v1/me/karma";
        }

        public static class User 
        {
            public static string Details(string username) => $"https://oauth.reddit.com/user/{username}/about";
            public static string History(string username, string type) => $"https://oauth.reddit.com/user/{username}/{type}";
        }

        public static class Subreddit 
        {
            public static string Details(string subredditName) => $"https://oauth.reddit.com/r/{subredditName}/about";
            public static string Submissions(string subredditName, string sort) 
                => $"https://oauth.reddit.com/r/{subredditName}/{sort}";
            public static string Subscription => "https://oauth.reddit.com/api/subscribe";
        }

        public static class Submission 
        {
            public static string Comments(string subredditName, string submissionId) =>
                $"https://oauth.reddit.com/r/{subredditName}/comments/{submissionId}";
        }

        public static class UserContent
        {
        public static string Vote => "https://oauth.reddit.com/api/vote/";  
        }
    }
}