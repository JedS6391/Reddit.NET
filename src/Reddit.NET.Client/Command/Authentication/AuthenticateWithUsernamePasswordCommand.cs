using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Reddit.NET.Client.Command.Authentication
{
    /// <summary>
    /// Defines a command to authenticate using the <c>password</c> grant type.
    /// </summary>
    public sealed class AuthenticateWithUsernamePasswordCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateWithUsernamePasswordCommand" /> class.
        /// </summary>
        public AuthenticateWithUsernamePasswordCommand(Parameters parameters)
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
                { "grant_type", "password" },
                { "username", _parameters.Username },
                { "password", _parameters.Password }
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
            /// Gets or sets the username.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the password.
            /// </summary>
            public string Password { get; set; }

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
