using Microsoft;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Context
{
    /// <summary>
    /// Defines an <see cref="AuthenticationContext" /> used when authenticating with <see cref="UserTokenAuthenticator" />.
    /// </summary>
    public sealed class UserTokenAuthenticationContext : AuthenticationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserTokenAuthenticationContext" /> class.
        /// </summary>
        /// <param name="token">The authentication token for this context.</param>
        public UserTokenAuthenticationContext(Token token)
            : base()
        {
            Token = Requires.NotNull(token, nameof(token));
        }

        /// <inheritdoc />
        public override string Id => "User Token";

        /// <inheritdoc />
        public override Token Token { get; }
    }
}
