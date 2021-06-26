using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Reddit.NET.Client.Command.Authentication
{
    /// <summary>
    /// Defines a command to authenticate using the <c>refresh_token</c> grant type.
    /// </summary>
    public class AuthenticateWithRefreshTokenCommand : ClientCommand        
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateWithRefreshTokenCommand" /> class.
        /// </summary>
        public AuthenticateWithRefreshTokenCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(AuthenticateWithUsernamePasswordCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", _parameters.RefreshToken },                
                { "duration", "permanent" }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.Authentication.Token),
                Content = new FormUrlEncodedContent(requestParameters)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(
                    $"{_parameters.ClientId}:{_parameters.ClientSecret}")));

            return request;            
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters 
        {
            /// <summary>
            /// Gets or sets the refresh token.
            /// </summary>
            public string RefreshToken { get; set; } 

            /// <summary>
            /// Gets or sets the client identifier.
            /// </summary>
            public string ClientId { get; set; }
            
            /// <summary>
            /// Gets or sets the client secret.
            /// </summary>
            public string ClientSecret { get; set; }
        }
    }
}