using Reddit.NET.Client.Command.Submissions;
using Reddit.NET.Client.Command.Subreddits;
using Reddit.NET.Client.Command.Users;
using Reddit.NET.Client.Command.UserContent;
using System.Linq;
using System.Net.Http;
using Reddit.NET.Client.Command.Multireddits;

namespace Reddit.NET.Client
{
    /// <summary>
    /// Defines constants used by the client.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The named used for <see cref="HttpClient" /> instances.
        /// </summary>
        public static string HttpClientName = $"Reddit.NET.Client";

        /// <summary>
        /// Constants used to determine reddit 'thing' kinds.
        /// </summary>
        public static class Kind
        {
            public const string Comment = "t1";
            public const string User = "t2";
            public const string Submission = "t3";
            public const string Message = "t4";
            public const string Subreddit = "t5";
            public const string MoreComments = "more";
        }
    }
}
