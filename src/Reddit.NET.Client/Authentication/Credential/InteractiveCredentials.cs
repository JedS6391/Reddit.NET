using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft;
using Reddit.NET.Client.Authentication.Abstract;
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
        /// <param name="sessionId">A unique key generated at the end of the interactive authentication process.</param>
        /// <param name="token">A token retrieved at the end of the interactive authentication process.</param>
        internal InteractiveCredentials(
            AuthenticationMode mode,
            string clientId,
            string clientSecret,
            Uri redirectUri,
            Guid sessionId,
            Token token)
            : base(mode, clientId, clientSecret, redirectUri: redirectUri)
        {
            SessionId = sessionId;
            Token = Requires.NotNull(token, nameof(token));
        }

        /// <summary>
        /// Gets the unique key generated at the end of the interactive authentication process.
        /// </summary>
        public Guid SessionId { get; }

        /// <summary>
        /// Gets the token retrieved at the end of the interactive authentication process.
        /// </summary>
        internal Token Token { get; }

        /// <summary>
        /// Provides the ability to configure the credentials used for interactive authentication.
        /// </summary>
        public sealed class Builder
        {
            private static readonly string[] s_scopes = new string[]
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
                "history",
                "flair",
                "creddits"
            };

            private readonly AuthenticationMode _mode;
            private readonly string _clientId;
            private readonly string _clientSecret;
            private readonly Uri _redirectUri;
            private readonly string _state;
            private string _code;
            private Guid? _sessionId;
            private Token _token;
            private Stage _stage;

            /// <summary>
            /// Initializes a new instance of the <see cref="Builder" /> class.
            /// </summary>
            /// <remarks>
            /// This constructor is intended to be used at the start of the interactive authentication process.
            ///
            /// The builder will execute a command to retrieve a token in order to build the credentials.
            ///
            /// The token obtained will be stored in token storage and associated with a session identifier for later usage.
            /// </remarks>
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

            /// <summary>
            /// Initializes a new instance of the <see cref="Builder" /> class.
            /// </summary>
            /// <remarks>
            /// This constructor is intended to be used when the interactive authentication process has been previously completed.
            ///
            /// The builder will query the token storage for a token associated with the session identifier provided to build the credentials.
            /// </remarks>
            /// <param name="mode">The mode of authentication this instance represents.</param>
            /// <param name="clientId">The client ID of the reddit app.</param>
            /// <param name="clientSecret">The client secret of the reddit app.</param>
            /// <param name="redirectUri">The URL that users will be redirected to when authorizing your application.</param>
            /// <param name="sessionId">A unique key generated at the end of the interactive authentication process.</param>
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
                _sessionId = sessionId;
                _stage = Stage.AuthorizedWithSessionId;
            }

            /// <summary>
            /// Gets the URI the user should be sent to in order to start the interactive authentication flow.
            /// </summary>
            public Uri GetAuthorizationUri()
            {
                if (_stage == Stage.AuthorizedWithSessionId)
                {
                    throw new InvalidOperationException("Builder has already been authorized for an existing session.");
                }

                return new Uri($"https://www.reddit.com/api/v1/authorize?client_id={HttpUtility.UrlEncode(_clientId)}&response_type=code&state={HttpUtility.UrlEncode(_state)}&redirect_uri={HttpUtility.UrlEncode(_redirectUri.AbsoluteUri)}&duration=permanent&scope={HttpUtility.UrlEncode(string.Join(' ', s_scopes))}");
            }

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
            /// Authenticates based on the builder configuration.
            /// </summary>
            /// <remarks>
            /// Depending on whether an authorization code has been provided or an existing session identifier, the authentication process will be differ.
            /// </remarks>
            /// <param name="commandExecutor">A <see cref="CommandExecutor" /> instance used for executing commands.</param>
            /// <param name="tokenStorage">An <see cref="ITokenStorage" /> instance used for managing tokens.</param>
            /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
            /// <returns>A task representing the asynchronous operation.</returns>
            public async Task AuthenticateAsync(CommandExecutor commandExecutor, ITokenStorage tokenStorage, CancellationToken cancellationToken = default)
            {
                switch (_stage)
                {
                    case Stage.AuthorizedWithCode:
                        await AuthenticateWithCodeAsync(commandExecutor, tokenStorage, cancellationToken).ConfigureAwait(false);
                        break;

                    case Stage.AuthorizedWithSessionId:
                        await AuthenticateWithSessionIdAsync(tokenStorage, cancellationToken).ConfigureAwait(false);
                        break;

                    default:
                        throw new InvalidOperationException("Builder must be authorized with a code or session ID before authentication can be performed.");
                };
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
                    _clientId,
                    _clientSecret,
                    _redirectUri,
                    _sessionId.Value,
                    _token);
            }

            private async Task AuthenticateWithCodeAsync(CommandExecutor commandExecutor, ITokenStorage tokenStorage, CancellationToken cancellationToken)
            {
                var parameters = new AuthenticateWithAuthorizationCodeCommand.Parameters()
                {
                    Code = _code,
                    RedirectUri = _redirectUri.AbsoluteUri,
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                };

                var authenticateCommand = new AuthenticateWithAuthorizationCodeCommand(parameters);

                var token = await commandExecutor.ExecuteCommandAsync<Token>(authenticateCommand, cancellationToken).ConfigureAwait(false);

                var sessionId = await tokenStorage.StoreTokenAsync(token, cancellationToken);

                _token = token;
                _sessionId = sessionId;
                _stage = Stage.Authenticated;
            }

            private async Task AuthenticateWithSessionIdAsync(ITokenStorage tokenStorage, CancellationToken cancellationToken)
            {
                var token = await tokenStorage.GetTokenAsync(_sessionId.Value, cancellationToken);

                if (token == null)
                {
                    throw new InvalidOperationException($"No existing token associated with session identifier '{_sessionId.Value}'.");
                }

                _token = token;
                _stage = Stage.Authenticated;
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
