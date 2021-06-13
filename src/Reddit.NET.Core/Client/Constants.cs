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
        /// Constants used in requests made to reddit.
        /// </summary>
        public static class Request 
        {
            /// <summary>
            /// The name the client will send in the <c>User-Agent</c> header when making HTTP requests.
            /// </summary>
            public const string ClientName = "Reddit.NET client";
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
                nameof(GetHotSubredditSubmissionsCommand),
                nameof(GetSubmissionCommentsCommand)
            };

            /// <summary>
            /// The supported commands when using a user authentication mode.
            /// </summary>
            public static string[] UserCommandIds = new string[]
            {
                nameof(GetSubredditDetailsCommand),
                nameof(GetHotSubredditSubmissionsCommand),
                nameof(GetUserDetailsCommand),
                nameof(GetUserSubredditsCommand),
                nameof(ApplyVoteToSubmissionCommand),
                nameof(GetSubmissionCommentsCommand)
            };
        }
    }
}