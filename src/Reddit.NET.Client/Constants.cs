using System.Net.Http;

namespace Reddit.NET.Client
{
    /// <summary>
    /// Defines constants used by the client.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The name used for <see cref="HttpClient" /> instances.
        /// </summary>
        public static string HttpClientName = $"Reddit.NET.Client";

        /// <summary>
        /// Constants used to determine reddit <i>thing</i> kinds.
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
