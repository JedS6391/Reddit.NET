using System;
using System.Threading.Tasks;
using Microsoft;
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
        private Func<CommandExecutor, Task<Credentials>> _builderFunc;

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

            _builderFunc = (commandExecutor) => Task.FromResult<Credentials>(new NonInteractiveCredentials(
                AuthenticationMode.Script,
                clientId,            
                clientSecret,
                username: username,
                password: password));
        }

        /// <summary>
        /// Configures the builder to create credentials for use in the context of a script or web app where read-only access is required.
        /// </summary>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="clientSecret">The client secret of the reddit app.</param>
        /// <param name="deviceId">The identifier of the device.</param>        
        public void ReadOnly(string clientId, string clientSecret, Guid deviceId)
        {
            Requires.NotNull(clientId, nameof(clientId));
            Requires.NotNull(clientSecret, nameof(clientSecret));

            _builderFunc = (commandExecutor) => Task.FromResult<Credentials>(new NonInteractiveCredentials(
                AuthenticationMode.ReadOnly,
                clientId,            
                clientSecret,
                deviceId: deviceId));
        }   

        /// <summary>
        /// Configures the builder to create credentials for use in the context of an installed application (e.g. user's phone) where read-only access is required.
        /// </summary>
        /// <param name="clientId">The client ID of the reddit app.</param>        
        /// <param name="deviceId">The identifier of the device.</param>        
        public void ReadOnlyInstalledApp(string clientId, Guid deviceId)
        {
            Requires.NotNull(clientId, nameof(clientId));        

            _builderFunc = (commandExecutor) => Task.FromResult<Credentials>(new NonInteractiveCredentials(
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
        /// Web applications are able to keep a secret and will use the authorization code grant type
        /// to request access to reddit on behalf of a user.
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

            _builderFunc = async (commandExecutor) =>
            {
                await interactiveCredentialsBuilder.AuthenticateAsync(commandExecutor).ConfigureAwait(false);

                return interactiveCredentialsBuilder.Build();
            };

            return interactiveCredentialsBuilder;
        }

        /// <summary>
        /// Configures the builder to create credentials for use in the context of an installed application, e.g. a user's phone.
        /// </summary>
        /// <remarks>
        /// Installed applications are not able to keep a secret and will use the authorization code grant type
        /// to request access to reddit on behalf of a user.
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

            _builderFunc = async (commandExecutor) =>
            {
                await interactiveCredentialsBuilder.AuthenticateAsync(commandExecutor).ConfigureAwait(false);

                return interactiveCredentialsBuilder.Build();
            };

            return interactiveCredentialsBuilder;
        }

        /// <summary>
        /// Creates a <see cref="Credentials" /> instance based on the builder configuration.
        /// </summary>
        /// <param name="commandExecutor">A <see cref="CommandExecutor" /> instance used when creating the credentials.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the credentials.</returns>
        internal async Task<Credentials> BuildCredentialsAsync(CommandExecutor commandExecutor) =>
            await _builderFunc.Invoke(commandExecutor).ConfigureAwait(false);
    }
}