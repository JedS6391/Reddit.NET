using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Subreddits;

namespace Reddit.NET.Core.Client.Authentication.Context
{
    /// <summary>
    /// Defines an <see cref="AuthenticationContext" /> used when authenticating with <see cref="ClientCredentialsAuthenticator" />.
    /// </summary>
    public sealed class ClientCredentialsAuthenticationContext : AuthenticationContext
    {
        private static readonly string[] _supportedCommandIds = new string[]
        {
            nameof(GetSubredditDetailsCommand)
        };

        /// <inheritdoc />
        public override string Id => "Client Credentials";

        /// <inheritdoc />
        public override Token Token { get; }        

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCredentialsAuthenticationContext" /> class.
        /// </summary>
        /// <param name="token">The authentication token for this context.</param>
        public ClientCredentialsAuthenticationContext(Token token) 
            : base(_supportedCommandIds)
        {
            Token = token;
        }
    }
}