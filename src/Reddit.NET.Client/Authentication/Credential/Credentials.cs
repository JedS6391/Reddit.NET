using System;
using Microsoft;

namespace Reddit.NET.Client.Authentication.Credential
{
    /// <summary>
    /// Encapsulates the available details used during authentication with reddit.
    /// </summary>
    public abstract class Credentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Credentials" /> class.
        /// </summary>
        /// <param name="mode">The mode of authentication this instance represents.</param>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="clientSecret">The client secret of the reddit app.</param>
        /// <param name="username">The username of the user the reddit app is for.</param>
        /// <param name="password">The password of the user the reddit app is for.</param>
        /// <param name="redirectUri">The URL that users will be redirected to when authorizing your application.</param>
        /// <param name="deviceId">The identifier of the device.</param>
        protected Credentials(
            AuthenticationMode mode,
            string clientId,
            string clientSecret,
            string username = null,
            string password = null,
            Uri redirectUri = null,
            Guid? deviceId = null)
        {
            Requires.NotNull(clientId, nameof(clientId));
            Requires.NotNull(clientSecret, nameof(clientSecret));

            Mode = mode;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Username = username;
            Password = password;
            RedirectUri = redirectUri;
            DeviceId = deviceId;
        }

        /// <summary>
        /// Gets the mode of authentication this instance represents.
        /// </summary>
        public AuthenticationMode Mode { get; }

        /// <summary>
        /// Gets the client ID of the reddit app.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Gets the client secret of the reddit app.
        /// </summary>
        public string ClientSecret { get; }

        /// <summary>
        /// Gets the username of the user the reddit app is for.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the password of the user the reddit app is for.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets the URL that users will be redirected to when authorizing your application
        /// </summary>
        public Uri RedirectUri { get; }

        /// <summary>
        /// Gets the identifier of the device.
        /// </summary>
        public Guid? DeviceId { get; }
    }
}
