using System;
using Microsoft;
using Reddit.NET.Core.Client.Authentication.Abstract;

namespace Reddit.NET.Core.Client.Authentication
{
    public class NonInteractiveCredentials : Credentials
    {
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