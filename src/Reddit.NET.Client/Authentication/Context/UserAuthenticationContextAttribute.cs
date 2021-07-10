using Reddit.NET.Client.Authentication.Abstract;

namespace Reddit.NET.Client.Authentication.Context
{
    /// <summary>
    /// Used to indicate that a user-based <see cref="AuthenticationContext" /> is supported.
    /// </summary>
    public sealed class UserAuthenticationContextAttribute : SupportedAuthenticationContextAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticationContextAttribute" /> class.
        /// </summary>
        public UserAuthenticationContextAttribute()
            : base(typeof(UsernamePasswordAuthenticationContext), typeof(UserTokenAuthenticationContext))
        {
        }
    }
}
