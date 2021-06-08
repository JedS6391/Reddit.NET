using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Submissions;
using Reddit.NET.Core.Client.Command.Subreddits;
using Reddit.NET.Core.Client.Command.Users;

namespace Reddit.NET.Core.Client
{
    public class UserRefreshTokenAuthenticationContext : AuthenticationContext
    {
        private static readonly string[] _supportedCommandIds = new string[]
        {
            nameof(GetSubredditDetailsCommand),
            nameof(GetHotSubredditSubmissionsCommand),
            nameof(GetUserDetailsCommand),
            nameof(GetUserSubredditsCommand),
            nameof(ApplyVoteToSubmissionCommand),
            nameof(GetSubmissionCommentsCommand)
        };

        public override string Id => "User Refresh Token";

        public override Token Token { get; }

        public UserRefreshTokenAuthenticationContext(Token token)
            : base(_supportedCommandIds)
        {
            Token = token;
        }
    }
}