using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Builder.Exceptions;
using Reddit.NET.Core.Client.Command;
using Reddit.NET.Core.Client.Command.Authentication;

namespace Reddit.NET.Core.Client.Builder
{
    /// <summary>
    /// Provides the ability to create <see cref="RedditClient" /> instances.
    /// </summary>
    public class RedditClientBuilder
    {
        private IHttpClientFactory _httpClientFactory;
        private ILoggerFactory _loggerFactory;
        private Action<CredentialsConfiguration> _credentialsConfigurationAction;


        /// <summary>
        /// Creates a new <see cref="RedditClientBuilder" /> instance to start the build process.
        /// </summary>
        public static RedditClientBuilder New => new RedditClientBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditClientBuilder" /> class.
        /// </summary>
        private RedditClientBuilder()
        {
        }

        /// <summary>
        /// Configures the builder to use the provided <see cref="ILoggerFactory" /> instance when logging messages.
        /// </summary>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory" /> instance.</param>
        /// <returns>The updated builder.</returns>
        public RedditClientBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            return this;
        }

        /// <summary>
        /// Configures the builder to use the provided <see cref="IHttpClientFactory" /> instance when making HTTP calls.
        /// </summary>
        /// <param name="httpClientFactory">A <see cref="IHttpClientFactory" /> instance.</param>
        /// <returns>The updated builder.</returns>
        public RedditClientBuilder WithHttpClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

            return this;
        }

        public RedditClientBuilder WithCredentials(Action<CredentialsConfiguration> configurationAction)
        {
            _credentialsConfigurationAction = configurationAction;

            return this;
        }

        /// <summary>
        /// Creates a <see cref="RedditClient" /> instance based on the builder configuration.
        /// </summary>
        /// <returns>A <see cref="RedditClient" /> instance configured based on the builder.</returns>
        public async Task<RedditClient> BuildAsync() 
        {
            CheckValidity();

            var commandExecutor = new CommandExecutor(
                _loggerFactory.CreateLogger<CommandExecutor>(),
                _httpClientFactory);

            var authenticatorFactory = new AuthenticatorFactory(
                _loggerFactory, 
                commandExecutor);

            var credentialsConfiguration = new CredentialsConfiguration();

            _credentialsConfigurationAction.Invoke(credentialsConfiguration);

            var credentials = await credentialsConfiguration.BuildCredentialsAsync(commandExecutor).ConfigureAwait(false);

            var authenticator = authenticatorFactory.GetAuthenticator(credentials);

            return new RedditClient(
                _loggerFactory.CreateLogger<RedditClient>(), 
                commandExecutor, 
                authenticator);
        }

        private void CheckValidity()
        {            
            if (_httpClientFactory == null) 
            {
                throw new RedditClientBuilderException("No HTTP client factory configured.");
            }

            if (_loggerFactory == null) 
            {
                throw new RedditClientBuilderException("No logger factory configured.");
            }

            if (_credentialsConfigurationAction == null)
            {
                throw new RedditClientBuilderException("No credentials configured.");
            }
        }

        public class CredentialsConfiguration
        {
            private Func<CommandExecutor, Task<Credentials>> _credentialsBuilder;

            internal CredentialsConfiguration()
            {                
            }

            public static CredentialsConfiguration New => new CredentialsConfiguration();

            /// <summary>
            /// Creates credentials for use in the context of a script.
            /// </summary>
            /// <remarks>
            /// Scripts are able to keep a secret and only have access to the account the client ID + secret is for.
            /// </remarks>
            /// <param name="clientId">The client ID of the reddit app.</param>
            /// <param name="clientSecret">The client secret of the reddit app.</param>
            /// <param name="username">The username of the user the reddit app is for.</param>
            /// <param name="password">The password of the user the reddit app is for.</param>
            /// <returns>A <see cref="NonInteractiveCredentials" /> instance representing the kind of credentials provided.</returns>
            public void Script(string clientId, string clientSecret, string username, string password)
            {
                Requires.NotNull(username, nameof(username));
                Requires.NotNull(password, nameof(password));

                _credentialsBuilder = (commandExecutor) => Task.FromResult<Credentials>(new NonInteractiveCredentials(
                    AuthenticationMode.Script,
                    clientId,            
                    clientSecret,
                    username: username,
                    password: password));
            }

            /// <summary>
            /// Creates credentials for use in the context of a script or web app where read-only access is required.
            /// </summary>
            /// <param name="clientId">The client ID of the reddit app.</param>
            /// <param name="clientSecret">The client secret of the reddit app.</param>
            /// <param name="deviceId">The identifier of the device.</param>
            /// <returns>A <see cref="NonInteractiveCredentials" /> instance representing the kind of credentials provided.</returns>
            public void ReadOnly(string clientId, string clientSecret, Guid deviceId)
            {
                _credentialsBuilder = (commandExecutor) => Task.FromResult<Credentials>(new NonInteractiveCredentials(
                    AuthenticationMode.ReadOnly,
                    clientId,            
                    clientSecret,
                    deviceId: deviceId));
            }   

            /// <summary>
            /// Creates credentials for use in the context of an installed application (e.g. user's phone) where read-only access is required.
            /// </summary>
            /// <param name="clientId">The client ID of the reddit app.</param>        
            /// <param name="deviceId">The identifier of the device.</param>
            /// <returns>A <see cref="NonInteractiveCredentials" /> instance representing the kind of credentials provided.</returns>
            public void ReadOnlyInstalledApp(string clientId, Guid deviceId)
            {
                _credentialsBuilder = (commandExecutor) => Task.FromResult<Credentials>(new NonInteractiveCredentials(
                    AuthenticationMode.ReadOnlyInstalledApp,
                    clientId,            
                    // No secret is expected for an installed app as the secret cannot be stored securely.
                    clientSecret: string.Empty,
                    deviceId: deviceId));
            }

            /// <summary>
            /// Creates credentials for use in the context of a web application.
            /// </summary>
            /// <remarks>
            /// Web applications are able to keep a secret and will use the authorization code grant type
            /// to request access to reddit on behalf of a user.
            /// </remarks>
            /// <param name="clientId">The client ID of the reddit app.</param>
            /// <param name="clientSecret">The client secret of the reddit app.</param>
            /// <param name="redirectUri">The URL that users will be redirected to when authorizing your application.</param>
            /// <returns>A <see cref="NonInteractiveCredentials" /> instance representing the kind of credentials provided.</returns>
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

                _credentialsBuilder = async (commandExecutor) =>
                {
                    await interactiveCredentialsBuilder.AuthenticateAsync(commandExecutor).ConfigureAwait(false);

                    return interactiveCredentialsBuilder.Build();
                };

                return interactiveCredentialsBuilder;
            }

            /// <summary>
            /// Creates credentials for use in the context of an installed application, e.g. a user's phone.
            /// </summary>
            /// <remarks>
            /// Installed applications are not able to keep a secret and will use the authorization code grant type
            /// to request access to reddit on behalf of a user.
            /// </remarks>
            /// <param name="clientId">The client ID of the reddit app.</param>
            /// <param name="redirectUri">The URL that users will be redirected to when authorizing your application.</param>
            /// <returns>A <see cref="NonInteractiveCredentials" /> instance representing the kind of credentials provided.</returns>
            public InteractiveCredentials.Builder InstalledApp(string clientId, Uri redirectUri, string state)
            {
                Requires.NotNull(redirectUri, nameof(redirectUri));

                var interactiveCredentialsBuilder = new InteractiveCredentials.Builder(
                    AuthenticationMode.InstalledApp,
                    clientId,
                    // No secret is expected for an installed app as the secret cannot be stored securely.
                    clientSecret: string.Empty,
                    redirectUri: redirectUri,
                    state: state);

                _credentialsBuilder = async (commandExecutor) =>
                {
                    await interactiveCredentialsBuilder.AuthenticateAsync(commandExecutor).ConfigureAwait(false);

                    return interactiveCredentialsBuilder.Build();
                };

                return interactiveCredentialsBuilder;
            }

            internal async Task<Credentials> BuildCredentialsAsync(CommandExecutor commandExecutor) =>
                await _credentialsBuilder.Invoke(commandExecutor).ConfigureAwait(false);
        }
    }
}