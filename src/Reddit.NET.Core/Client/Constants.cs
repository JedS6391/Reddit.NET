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
            public const string ClientName = "Reddit.NET client";
        }

        /// <summary>
        /// Constants used in commands.
        /// </summary>
        public static class Command 
        {
            public static string[] ReadOnlyCommandIds = new string[]
            {
                nameof(GetSubredditDetailsCommand),
                nameof(GetHotSubredditSubmissionsCommand),
                nameof(GetSubmissionCommentsCommand)
            };

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