using Reddit.NET.Core.Client.Command.Submissions;
using Reddit.NET.Core.Client.Command.Subreddits;
using Reddit.NET.Core.Client.Command.Users;

namespace Reddit.NET.Core.Client
{
    /// <summary>
    /// Defines constants used by the client.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The version of the client.
        /// </summary>
        public const string Version = "0.1.0";

        /// <summary>
        /// Constants used in requests made to reddit.
        /// </summary>
        public static class Request 
        {
            /// <summary>
            /// The name the client will send in the <c>User-Agent</c> header when making HTTP requests.
            /// </summary>
            public static string ClientName = $"Reddit.NET.Core.Client v{Version} (by JedS6391)";
        }

        /// <summary>
        /// Constants used in commands.
        /// </summary>
        public static class Command 
        {
            /// <summary>
            /// The supported commands when using a read-only authentication mode.
            /// </summary>
            public static string[] ReadOnlyCommandIds = new string[]
            {
                nameof(GetSubredditDetailsCommand),
                nameof(GetSubredditSubmissionsCommand),
                nameof(GetSubmissionCommentsCommand)
            };

            /// <summary>
            /// The supported commands when using a user authentication mode.
            /// </summary>
            public static string[] UserCommandIds = new string[]
            {
                nameof(GetSubredditDetailsCommand),
                nameof(GetSubredditSubmissionsCommand),
                nameof(GetMyDetailsCommand),
                nameof(GetUserSubredditsCommand),
                nameof(ApplyVoteToSubmissionCommand),
                nameof(GetSubmissionCommentsCommand)
            };
        }
    }
}