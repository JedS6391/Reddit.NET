using Reddit.NET.Client.Authentication.Abstract;

namespace Reddit.NET.Client.Authentication.Context
{
    /// <summary>
    /// Used to indicate that a read-only <see cref="AuthenticationContext" /> is supported.
    /// </summary>
    public sealed class ReadOnlyAuthenticationContextAttribute : SupportedAuthenticationContextAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyAuthenticationContextAttribute" /> class.
        /// </summary>
        public ReadOnlyAuthenticationContextAttribute()
            : base(typeof(ClientCredentialsAuthenticationContext), typeof(InstalledClientAuthenticationContext))
        {
        }
    }
}
