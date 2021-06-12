using System;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Authentication;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Authentication
{
    public class InteractiveCredentials : Credentials
    {
        internal InteractiveCredentials(
            AuthenticationMode mode, 
            string clientId, 
            string clientSecret, 
            Uri redirectUri,
            string state,
            Token token)
            : base(mode, clientId, clientSecret, redirectUri: redirectUri)
        {
            State = state;
            Token = token;
        }

        internal string State { get; }
        internal Token Token { get; }

        public class Builder
        {
            private static readonly string[] Scopes = new string[]
            {        
                "subscribe",            
                "vote",            
                "mysubreddits",
                "submit",
                "save",
                "read",
                "privatemessages",
                "identity",
                "account",
                "edit",
                "history"
            };

            private readonly AuthenticationMode _mode;
            private readonly string _clientId;
            private readonly string _clientSecret;
            private readonly Uri _redirectUri;
            private readonly string _state;
            private string _code;
            private Token _token;

            internal Builder(
                AuthenticationMode mode, 
                string clientId, 
                string clientSecret, 
                Uri redirectUri,
                string state)
            {
                _mode = mode;
                _clientId = clientId;
                _clientSecret = clientSecret;
                _redirectUri = redirectUri;
                _state = state;
            }

            public Uri AuthorizationUri =>
                new Uri($"https://www.reddit.com/api/v1/authorize?client_id={_clientId}&response_type=code&state={_state}&redirect_uri={_redirectUri}&duration=permanent&scope={string.Join(' ', Scopes)}");

            public void Authorize(string code) 
            {
                _code = code;
            }

            internal async Task AuthenticateAsync(CommandExecutor commandExecutor)
            {
                var parameters = new AuthenticateWithAuthorizationCodeCommand.Parameters()
                {
                    Code = _code,
                    RedirectUri = _redirectUri.AbsoluteUri,
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                };

                var authenticateCommand = new AuthenticateWithAuthorizationCodeCommand(parameters);

                _token = await commandExecutor.ExecuteCommandAsync<Token>(authenticateCommand).ConfigureAwait(false);                
            }

            internal InteractiveCredentials Build() => new InteractiveCredentials(
                _mode,
                _clientId,
                _clientSecret,
                _redirectUri,
                _state,
                _token);
        }   
    }
}