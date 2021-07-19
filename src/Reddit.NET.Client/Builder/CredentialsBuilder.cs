using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Authentication.Credential;
using Reddit.NET.Client.Command;

namespace Reddit.NET.Client.Builder
{
    /// <summary>
    /// Provides the ability to configure the credentials used by <see cref="RedditClient" />.
    /// </summary>
    /// <remarks>
    /// See <see cref="AuthenticationMode" /> for more details on the types of credentials.
    /// </remarks>
    public sealed class CredentialsBuilder
    {
        private Func<CommandExecutor, ITokenStorage, CancellationToken, Task<Credentials>> _builderFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialsBuilder" /> class.
        /// </summary>
        internal CredentialsBuilder()
        {
        }

        /// <summary>
        /// Creates a new <see cref="CredentialsBuilder" /> instance to start the build process.
        /// </summary>
        public static CredentialsBuilder New => new CredentialsBuilder();

        /// <summary>
        /// Configures the builder to create credentials for use in the context of a script.
        /// </summary>
        /// <remarks>
        /// Scripts are able to keep a secret and only have access to the account the client ID and secret is for.
        /// </remarks>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="clientSecret">The client secret of the reddit app.</param>
        /// <param name="username">The username of the user the reddit app is for.</param>
        /// <param name="password">The password of the user the reddit app is for.</param>
        public void Script(string clientId, string clientSecret, string username, string password)
        {
            Requires.NotNull(clientId, nameof(clientId));
            Requires.NotNull(clientSecret, nameof(clientSecret));
            Requires.NotNull(username, nameof(username));
            Requires.NotNull(password, nameof(password));

            _builderFunc = (_, _, _) => Task.FromResult<Credentials>(new NonInteractiveCredentials(
                AuthenticationMode.Script,
                clientId,
                clientSecret,
                username: username,
                password: password));
        }

        /// <summary>
        /// Configures the builder to create credentials for use in the context of a script or web app
        /// where read-only access is required.
        /// </summary>
        /// <remarks>
        /// When using read-only authentication, operations that require user-based authentication will not be supported.
        /// </remarks>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="clientSecret">The client secret of the reddit app.</param>
        /// <param name="deviceId">The identifier of the device.</param>
        public void ReadOnly(string clientId, string clientSecret, Guid deviceId)
        {
            Requires.NotNull(clientId, nameof(clientId));
            Requires.NotNull(clientSecret, nameof(clientSecret));

            _builderFunc = (_, _, _) => Task.FromResult<Credentials>(new NonInteractiveCredentials(
                AuthenticationMode.ReadOnly,
                clientId,
                clientSecret,
                deviceId: deviceId));
        }

        /// <summary>
        /// Configures the builder to create credentials for use in the context of an installed application (e.g. user's phone)
        /// where read-only access is required.
        /// </summary>
        /// <remarks>
        /// When using read-only authentication, operations that require user-based authentication will not be supported.
        /// </remarks>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="deviceId">The identifier of the device.</param>
        public void ReadOnlyInstalledApp(string clientId, Guid deviceId)
        {
            Requires.NotNull(clientId, nameof(clientId));

            _builderFunc = (_, _, _) => Task.FromResult<Credentials>(new NonInteractiveCredentials(
                AuthenticationMode.ReadOnlyInstalledApp,
                clientId,
                // No secret is expected for an installed app as the secret cannot be stored securely.
                clientSecret: string.Empty,
                deviceId: deviceId));
        }

        /// <summary>
        /// Configures the builder to create credentials for use in the context of a web application.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Web applications are able to keep a secret and will use the authorization code grant type
        /// to request access to reddit on behalf of a user.
        /// </para>
        /// <para>
        /// Web app credentials are considered <i>interactive</i> and require further user interaction to be configured.
        ///
        /// The interactive configuration can be managed by the returned <see cref="InteractiveCredentials.Builder" />.
        /// </para>
        /// <para>
        /// Upon creation, the builder will asynchronously complete the authentication flow to create the appropriate credentials.
        /// </para>
        /// </remarks>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="clientSecret">The client secret of the reddit app.</param>
        /// <param name="redirectUri">The URL that users will be redirected to when authorizing your application.</param>
        /// <param name="state">A randomly generated value to use during the authorization flow.</param>
        /// <returns>A <see cref="InteractiveCredentials.Builder" /> instance to further configure the interactive credentials.</returns>
        public InteractiveCredentials.Builder WebApp(
            string clientId,
            string clientSecret,
            Uri redirectUri,
            string state)
        {
            Requires.NotNull(clientId, nameof(clientId));
            Requires.NotNull(clientSecret, nameof(clientSecret));
            Requires.NotNull(redirectUri, nameof(redirectUri));
            Requires.NotNull(state, nameof(state));

            var interactiveCredentialsBuilder = new InteractiveCredentials.Builder(
                AuthenticationMode.WebApp,
                clientId,
                clientSecret,
                redirectUri,
                state);

            _builderFunc = async (commandExecutor, tokenStorage, cancellationToken) =>
            {
                await interactiveCredentialsBuilder
                    .AuthenticateAsync(commandExecutor, tokenStorage, cancellationToken)
                    .ConfigureAwait(false);

                return interactiveCredentialsBuilder.Build();
            };

            return interactiveCredentialsBuilder;
        }

        /// <summary>
        /// Configures the builder to create credentials for use in the context of an installed application, e.g. a user's phone.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Installed applications are not able to keep a secret and will use the authorization code grant type
        /// to request access to reddit on behalf of a user.
        /// </para>
        /// <para>
        /// Installed app credentials are considered <i>interactive</i> and require further user interaction to be configured.
        ///
        /// The interactive configuration can be managed by the returned <see cref="InteractiveCredentials.Builder" />.
        /// </para>
        /// <para>
        /// Upon creation, the builder will asynchronously complete the authentication flow to create the appropriate credentials.
        /// </para>
        /// </remarks>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="redirectUri">The URL that users will be redirected to when authorizing your application.</param>
        /// <param name="state">A randomly generated value to use during the authorization flow.</param>
        /// <returns>A <see cref="InteractiveCredentials.Builder" /> instance to further configure the interactive credentials.</returns>
        public InteractiveCredentials.Builder InstalledApp(string clientId, Uri redirectUri, string state)
        {
            Requires.NotNull(clientId, nameof(clientId));
            Requires.NotNull(redirectUri, nameof(redirectUri));
            Requires.NotNull(state, nameof(state));

            var interactiveCredentialsBuilder = new InteractiveCredentials.Builder(
                AuthenticationMode.InstalledApp,
                clientId,
                // No secret is expected for an installed app as the secret cannot be stored securely.
                clientSecret: string.Empty,
                redirectUri: redirectUri,
                state: state);

            _builderFunc = async (commandExecutor, tokenStorage, cancellationToken) =>
            {
                await interactiveCredentialsBuilder
                    .AuthenticateAsync(commandExecutor, tokenStorage, cancellationToken)
                    .ConfigureAwait(false);

                return interactiveCredentialsBuilder.Build();
            };

            return interactiveCredentialsBuilder;
        }

        /// <summary>
        /// Configures the builder to create credentials for use when an existing session identifier is available.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A session identifier can be obtained at the end of the interactive authentication process (<see cref="WebApp" /> or <see cref="InstalledApp" />).
        /// </para>
        /// <para>
        /// It can then be used to create interactive credentials without restarting the interaction authentication process.
        /// </para>
        /// </remarks>
        /// <param name="mode">The </param>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="clientSecret">The client secret of the reddit app.</param>
        /// <param name="redirectUri">The URL that users will be redirected to when authorizing your application.</param>
        /// <param name="sessionId">A unique key generated at the end of the interactive authentication process.</param>
        /// <returns>A <see cref="InteractiveCredentials.Builder" /> instance to further configure the interactive credentials.</returns>
        public InteractiveCredentials.Builder Session(
            AuthenticationMode mode,
            string clientId,
            string clientSecret,
            Uri redirectUri,
            Guid sessionId)
        {
            Requires.Argument(
                mode == AuthenticationMode.WebApp || mode == AuthenticationMode.InstalledApp,
                nameof(mode),
                "Mode must be web-app or installed app.");
            Requires.NotNull(clientId, nameof(clientId));
            Requires.NotNull(clientSecret, nameof(clientSecret));
            Requires.NotNull(redirectUri, nameof(redirectUri));

            var interactiveCredentialsBuilder = new InteractiveCredentials.Builder(
                mode,
                clientId,
                clientSecret,
                redirectUri,
                sessionId);

            _builderFunc = async (commandExecutor, tokenStorage, cancellationToken) =>
            {
                await interactiveCredentialsBuilder
                    .AuthenticateAsync(commandExecutor, tokenStorage, cancellationToken)
                    .ConfigureAwait(false);

                return interactiveCredentialsBuilder.Build();
            };

            return interactiveCredentialsBuilder;
        }

        /// <summary>
        /// Creates a <see cref="Credentials" /> instance based on the builder configuration.
        /// </summary>
        /// <param name="commandExecutor">A <see cref="CommandExecutor" /> instance used when creating the credentials.</param>
        /// <param name="tokenStorage">An <see cref="ITokenStorage" /> instance used for managing tokens.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the credentials.</returns>
        public async Task<Credentials> BuildCredentialsAsync(
            CommandExecutor commandExecutor,
            ITokenStorage tokenStorage,
            CancellationToken cancellationToken = default) =>
                await _builderFunc.Invoke(commandExecutor, tokenStorage, cancellationToken).ConfigureAwait(false);
    }
}
