using System;
using Microsoft;

namespace Reddit.NET.Core.Client.Authentication.Abstract
{
    public abstract class Credentials
    {
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

        public AuthenticationMode Mode { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string Username { get; }
        public string Password { get; }
        public Uri RedirectUri { get; }
        public Guid? DeviceId { get; }
    }
}