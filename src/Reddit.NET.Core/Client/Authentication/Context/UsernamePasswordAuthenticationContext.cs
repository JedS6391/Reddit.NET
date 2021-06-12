using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Submissions;
using Reddit.NET.Core.Client.Command.Subreddits;
using Reddit.NET.Core.Client.Command.Users;

namespace Reddit.NET.Core.Client.Authentication.Context
{
    /// <summary>
    /// Defines an <see cref="AuthenticationContext" /> used when authenticating with <see cref="UsernamePasswordAuthenticator" />.
    /// </summary>
    public sealed class UsernamePasswordAuthenticationContext : AuthenticationContext
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

        /// <inheritdoc />
        public override string Id => "Username + Password";

        /// <inheritdoc />
        public override Token Token { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsernamePasswordAuthenticationContext" /> class.
        /// </summary>
        /// <param name="token">The authentication token for this context.</param>
        public UsernamePasswordAuthenticationContext(Token token)
            : base(_supportedCommandIds)
        {
            Token = token;
        }
    }
}