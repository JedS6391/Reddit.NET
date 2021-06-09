using System;
using Microsoft;
using Reddit.NET.Core.Client.Authentication.Abstract;

namespace Reddit.NET.Core.Client.Authentication
{
    public class NonInteractiveCredentials : Credentials
    {
        public NonInteractiveCredentials(
            AuthenticationMode mode, 
            string clientId, 
            string clientSecret, 
            string username = null, 
            string password = null,
            Guid? deviceId = null) 
                : base(mode, clientId, clientSecret, username: username, password: password, deviceId: deviceId)
        {
        }

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
        public static NonInteractiveCredentials Script(string clientId, string clientSecret, string username, string password)
        {
            Requires.NotNull(username, nameof(username));
            Requires.NotNull(password, nameof(password));

            return new NonInteractiveCredentials(
                AuthenticationMode.Script,
                clientId,            
                clientSecret,
                username: username,
                password: password);  
        }

        /// <summary>
        /// Creates credentials for use in the context of a script or web app where read-only access is required.
        /// </summary>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="clientSecret">The client secret of the reddit app.</param>
        /// <param name="deviceId">The identifier of the device.</param>
        /// <returns>A <see cref="NonInteractiveCredentials" /> instance representing the kind of credentials provided.</returns>
        public static NonInteractiveCredentials ReadOnly(string clientId, string clientSecret, Guid deviceId)
        {
               return new NonInteractiveCredentials(
                    AuthenticationMode.ReadOnly,
                    clientId,            
                    clientSecret,
                    deviceId: deviceId);          
        }

        /// <summary>
        /// Creates credentials for use in the context of an installed application (e.g. user's phone) where read-only access is required.
        /// </summary>
        /// <param name="clientId">The client ID of the reddit app.</param>        
        /// <param name="deviceId">The identifier of the device.</param>
        /// <returns>A <see cref="NonInteractiveCredentials" /> instance representing the kind of credentials provided.</returns>
        public static NonInteractiveCredentials ReadOnlyInstalledApp(string clientId, Guid deviceId)
        {
               return new NonInteractiveCredentials(
                    AuthenticationMode.ReadOnlyInstalledApp,
                    clientId,            
                    // No secret is expected for an installed app as the secret cannot be stored securely.
                    clientSecret: string.Empty,
                    deviceId: deviceId);          
        }
    }
}