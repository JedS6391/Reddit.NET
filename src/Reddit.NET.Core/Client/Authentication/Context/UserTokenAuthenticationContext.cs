using Microsoft;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Authentication.Context
{
    /// <summary>
    /// Defines an <see cref="AuthenticationContext" /> used when authenticating with <see cref="UserTokenAuthenticator" />.
    /// </summary>
    public sealed class UserTokenAuthenticationContext : AuthenticationContext
    {
        private static readonly string[] _supportedCommandIds = Constants.Command.UserCommandIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTokenAuthenticationContext" /> class.
        /// </summary>
        /// <param name="token">The authentication token for this context.</param>
        public UserTokenAuthenticationContext(Token token)
            : base(_supportedCommandIds)
        {
            Token = Requires.NotNull(token, nameof(token));
        }

        /// <inheritdoc />
        public override string Id => "User Refresh Token";
        
        /// <inheritdoc />
        public override Token Token { get; }
    }
}