using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Subreddits;

namespace Reddit.NET.Core.Client.Authentication
{
    public class ClientCredentialsAuthenticationContext : AuthenticationContext
    {
        private static readonly string[] _supportedCommandIds = new string[]
        {
            nameof(GetSubredditDetailsCommand)
        };

        public override string Id => "Client Credentials";

        public override Token Token { get; }        

        public ClientCredentialsAuthenticationContext(Token token) 
            : base(_supportedCommandIds)
        {
            Token = token;
        }
    }
}