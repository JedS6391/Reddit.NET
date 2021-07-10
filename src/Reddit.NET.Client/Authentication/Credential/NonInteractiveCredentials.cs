using System;

namespace Reddit.NET.Client.Authentication.Credential
{
    /// <summary>
    /// A <see cref="Credentials" /> implementation used for non-interactive authentication.
    /// </summary>
    public sealed class NonInteractiveCredentials : Credentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonInteractiveCredentials" /> class.
        /// </summary>
        /// <param name="mode">The mode of authentication this instance represents.</param>
        /// <param name="clientId">The client ID of the reddit app.</param>
        /// <param name="clientSecret">The client secret of the reddit app.</param>
        /// <param name="username">The username of the user the reddit app is for.</param>
        /// <param name="password">The password of the user the reddit app is for.</param>
        /// <param name="deviceId">The identifier of the device.</param>
        internal NonInteractiveCredentials(
            AuthenticationMode mode,
            string clientId,
            string clientSecret,
            string username = null,
            string password = null,
            Guid? deviceId = null)
                : base(mode, clientId, clientSecret, username: username, password: password, deviceId: deviceId)
        {
        }
    }
}
