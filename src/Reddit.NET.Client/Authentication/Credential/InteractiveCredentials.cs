using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Builder;
using Reddit.NET.Client.Command;
using Reddit.NET.Client.Command.Authentication;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Credential
{
    /// <summary>
    /// A <see cref="Credentials" /> implementation used for interactive authentication.
    /// </summary>
    public sealed class InteractiveCredentials : Credentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonInteractiveCredentials" /> class.
        /// </summary>
        /// <param name="mode">The mode of authentication this instance represents.</param>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="clientSecret">The client secret of the reddit app.</param> 
        /// <param name="redirectUri">The URL that users will be redirected to when authorizing your application.</param>
        /// <param name="state">A randomly generated value to use during the authorization flow.</param>
        /// <param name="token">A token retrieved at the end of the interactive authentication.</param>
        internal InteractiveCredentials(
            AuthenticationMode mode, 
            Guid sessionId,
            string clientId, 
            string clientSecret, 
            Uri redirectUri,
            string state,
            Token token)
            : base(mode, clientId, clientSecret, redirectUri: redirectUri)
        {
            SessionId = sessionId;
            State = state;
            Token = Requires.NotNull(token, nameof(token));
        }

        public Guid SessionId { get; }

        /// <summary>
        /// Gets the state value used during the authorization flow.
        /// </summary>
        internal string State { get; }

        /// <summary>
        /// Gets the token retrieved at the end of the interactive authentication.
        /// </summary>
        internal Token Token { get; }

        /// <summary>
        /// Provides the ability to configure the credentials used for interactive authentication.
        /// </summary>
        public sealed class Builder
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
            private Stage _stage;

            /// <summary>
            /// Initializes a new instance of the <see cref="InteractiveCredentials.Builder" /> class.
            /// </summary>
            /// <param name="mode">The mode of authentication this instance represents.</param>
            /// <param name="clientId">The client ID of the reddit app.</param>
            /// <param name="clientSecret">The client secret of the reddit app.</param> 
            /// <param name="redirectUri">The URL that users will be redirected to when authorizing your application.</param>
            /// <param name="state">A randomly generated value to use during the authorization flow.</param>
            internal Builder(
                AuthenticationMode mode, 
                string clientId, 
                string clientSecret, 
                Uri redirectUri,
                string state)
            {
                _mode = mode;
                _clientId = Requires.NotNull(clientId, nameof(clientId));
                _clientSecret = Requires.NotNull(clientSecret, nameof(clientSecret));
                _redirectUri = Requires.NotNull(redirectUri, nameof(redirectUri));
                _state = Requires.NotNull(state, nameof(state));            
                _stage = Stage.Initialised;                            
            }

            internal Builder(
                AuthenticationMode mode,
                string clientId,
                string clientSecret,
                Uri redirectUri,
                Guid sessionId)
            {
                _mode = mode;
                _clientId = Requires.NotNull(clientId, nameof(clientId));
                _clientSecret = Requires.NotNull(clientSecret, nameof(clientSecret));
                _redirectUri = Requires.NotNull(redirectUri, nameof(redirectUri));                
                _stage = Stage.AuthorizedWithSessionId;

                SessionId = sessionId;
            }

            /// <summary>
            /// Gets the URI the user should be sent to in order to start the interactive authentication flow.
            /// </summary>
            public Uri AuthorizationUri =>
                new Uri($"https://www.reddit.com/api/v1/authorize?client_id={HttpUtility.UrlEncode(_clientId)}&response_type=code&state={HttpUtility.UrlEncode(_state)}&redirect_uri={HttpUtility.UrlEncode(_redirectUri.AbsoluteUri)}&duration=permanent&scope={HttpUtility.UrlEncode(string.Join(' ', Scopes))}");

            public Guid? SessionId { get; private set; }

            /// <summary>
            /// Completes the interactive authentication flow.
            /// </summary>
            /// <param name="code">The authorization code returned on completion of interactive authentication.</param>
            public void Authorize(string code) 
            {
                if (_stage != Stage.Initialised)
                {
                    throw new InvalidOperationException("Builder has already been authorized.");
                }

                _code = code;
                _stage = Stage.AuthorizedWithCode;
            }

            /// <summary>
            /// Authenticates based on the builder configuration to obtain a token.
            /// </summary>
            /// <param name="commandExecutor">A <see cref="CommandExecutor" /> instance used for executing commands.</param>
            /// <returns>A task representing the asynchronous operation.</returns>
            public async Task AuthenticateAsync(CommandExecutor commandExecutor, ITokenStorage tokenStorage)
            {
                if (_stage != Stage.AuthorizedWithCode && _stage != Stage.AuthorizedWithSessionId)
                {
                    throw new InvalidOperationException("Builder must be authorized with a code or session ID before authentication can be performed.");
                }

                if (_stage == Stage.AuthorizedWithCode)
                {
                    // Authorized with code
                    var parameters = new AuthenticateWithAuthorizationCodeCommand.Parameters()
                    {
                        Code = _code,
                        RedirectUri = _redirectUri.AbsoluteUri,
                        ClientId = _clientId,
                        ClientSecret = _clientSecret
                    };

                    var authenticateCommand = new AuthenticateWithAuthorizationCodeCommand(parameters);

                    _token = await commandExecutor.ExecuteCommandAsync<Token>(authenticateCommand).ConfigureAwait(false);
                    _stage = Stage.Authenticated;

                    SessionId = Guid.NewGuid();

                    await tokenStorage.StoreTokenAsync(SessionId.Value, _token);
                }
                else 
                {
                    // Authorized with session ID
                    _token = await tokenStorage.GetTokenAsync(SessionId.Value);
                    _stage = Stage.Authenticated;
                }
            }
            
            /// <summary>
            /// Builds an <see cref="InteractiveCredentials" /> instance based on the builder configuration.
            /// </summary>
            /// <returns>An <see cref="InteractiveCredentials" /> instance representing the builder configuration.</returns>
            public InteractiveCredentials Build()
            {
                if (_stage != Stage.Authenticated)
                {
                    throw new InvalidOperationException("Builder must be authenticated before credentials can be built.");
                }

                 return new InteractiveCredentials(
                    _mode,
                    SessionId.Value,
                    _clientId,
                    _clientSecret,
                    _redirectUri,
                    _state,
                    _token);
            }

            private enum Stage 
            {
                Initialised,
                AuthorizedWithCode,
                AuthorizedWithSessionId,
                Authenticated
            }
        }   
    }
}