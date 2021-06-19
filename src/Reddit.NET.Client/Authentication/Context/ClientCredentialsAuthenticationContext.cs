using Microsoft;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Context
{
    /// <summary>
    /// Defines an <see cref="AuthenticationContext" /> used when authenticating with <see cref="ClientCredentialsAuthenticator" />.
    /// </summary>
    public sealed class ClientCredentialsAuthenticationContext : AuthenticationContext
    {
        private static readonly string[] _supportedCommandIds = Constants.Command.ReadOnlyCommandIds;       

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCredentialsAuthenticationContext" /> class.
        /// </summary>
        /// <param name="token">The authentication token for this context.</param>
        public ClientCredentialsAuthenticationContext(Token token) 
            : base(_supportedCommandIds)
        {
            Token = Requires.NotNull(token, nameof(token));
        }

        /// <inheritdoc />
        public override string Id => "Client Credentials";

        /// <inheritdoc />
        public override Token Token { get; } 
    }
}