using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Reddit.NET.Client.Command.Authentication
{
    /// <summary>
    /// Defines a command to authenticate using the <c>authorization_code</c> grant type.
    /// </summary>
    public class AuthenticateWithAuthorizationCodeCommand : ClientCommand
    {
        private readonly AuthenticateWithAuthorizationCodeCommand.Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateWithAuthorizationCodeCommand" /> class.
        /// </summary>
        public AuthenticateWithAuthorizationCodeCommand(AuthenticateWithAuthorizationCodeCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(AuthenticateWithAuthorizationCodeCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", _parameters.Code },   
                { "redirect_uri", _parameters.RedirectUri },             
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
            /// Gets or sets the authorization code.
            /// </summary>
            public string Code { get; set; }     
            
            /// <summary>
            /// Gets or sets the redirect URI.
            /// </summary>
            public string RedirectUri { get; set; }

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