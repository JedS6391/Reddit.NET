using Reddit.NET.Client.Command.Submissions;
using Reddit.NET.Client.Command.Subreddits;
using Reddit.NET.Client.Command.Users;
using Reddit.NET.Client.Command.UserContent;
using System.Linq;

namespace Reddit.NET.Client
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
            public static string ClientName = $"Reddit.NET.Client v{Version} (by JedS6391)";
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
                nameof(SearchSubredditSubmissionsCommand),
                nameof(GetFrontPageSubmissionsCommand),
                nameof(GetSubmissionDetailsWithCommentsCommand),
                nameof(GetUserDetailsCommand),
                nameof(GetUserTrophiesCommand)
            };

            /// <summary>
            /// The supported commands when using a user authentication mode.
            /// </summary>
            public static string[] UserCommandIds = ReadOnlyCommandIds
                .Union(new string[]
                {                                
                    nameof(UpdateSubredditSubscriptionCommand),
                    nameof(CreateSubredditSubmissionCommand),                    
                    nameof(GetMyDetailsCommand),
                    nameof(GetMySubredditsCommand),
                    nameof(GetMyKarmaBreakdownCommand),
                    nameof(GetMyTrophiesCommand),
                    nameof(GetMyInboxMessagesCommand),                    
                    nameof(SendMessageCommand),
                    nameof(GetUserHistoryCommand),
                    nameof(ApplyVoteCommand),
                    nameof(SaveOrUnsaveContentCommand),
                    nameof(DeleteContentCommand),
                    nameof(CreateCommentCommand)
                })
                .ToArray();
        }

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