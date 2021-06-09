using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using Microsoft;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Authentication
{
    public class InteractiveCredentials : Credentials
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

        private InteractiveCredentials(
            AuthenticationMode mode, 
            string clientId, 
            string clientSecret, 
            Uri redirectUri)
            : base(mode, clientId, clientSecret, redirectUri: redirectUri)
        {
            State = GetRandomState();
        }

        public Uri AuthorizationUri =>
            new Uri($"https://www.reddit.com/api/v1/authorize?client_id={ClientId}&response_type=code&state={State}&redirect_uri={RedirectUri}&duration=permanent&scope={string.Join(' ', Scopes)}");

        public Token Token { get; internal set; }

        internal string State { get; }

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
        public static InteractiveCredentialsBuilder WebApp(
            string clientId, 
            string clientSecret, 
            Uri redirectUri) 
        {
            Requires.NotNull(redirectUri, nameof(redirectUri));

            var credentials = new InteractiveCredentials(
                AuthenticationMode.WebApp,
                clientId,
                clientSecret,
                redirectUri: redirectUri);

            return new InteractiveCredentialsBuilder(credentials);
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
        public static InteractiveCredentialsBuilder InstalledApp(string clientId, Uri redirectUri)
        {
            Requires.NotNull(redirectUri, nameof(redirectUri));

            var credentials = new InteractiveCredentials(
                AuthenticationMode.InstalledApp,
                clientId,
                // No secret is expected for an installed app as the secret cannot be stored securely.
                clientSecret: string.Empty,
                redirectUri: redirectUri); 

            return new InteractiveCredentialsBuilder(credentials);           
        }

        private static string GetRandomState() 
        {
            using var random = new RNGCryptoServiceProvider();

            byte[] tokenData = new byte[32];
            
            random.GetBytes(tokenData);

            return Convert.ToBase64String(tokenData);
        } 

        public class InteractiveCredentialsBuilder
        {
            private readonly InteractiveCredentials _credentials;

            internal InteractiveCredentialsBuilder(InteractiveCredentials credentials)
            {
                _credentials = credentials;
            }

            public Uri AuthorizationUri => _credentials.AuthorizationUri;

            public async Task<InteractiveCredentials> AuthorizeAsync(Uri finalRedirectUri)
            {
                var queryParameters = HttpUtility.ParseQueryString(finalRedirectUri.Query);

                var state = queryParameters.Get("state");
                var code = queryParameters.Get("code");

                if (string.IsNullOrEmpty(state) || state != _credentials.State)
                {
                    throw new InvalidOperationException("");
                }

                if (string.IsNullOrEmpty(code))
                {
                    throw new InvalidOperationException("");
                }

                // TODO: Authorization request
                var token = new Token();

                _credentials.Token = token;

                return _credentials;
            }
        }   
    }
}