using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Reddit.NET.Client.Command.Authentication
{
    /// <summary>
    /// Defines a command to authenticate using the <c>https://oauth.reddit.com/grants/installed_client</c> grant type.
    /// </summary>
    public sealed class AuthenticateWithInstalledClientCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateWithInstalledClientCommand" /> class.
        /// </summary>
        public AuthenticateWithInstalledClientCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(AuthenticateWithInstalledClientCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "grant_type", "https://oauth.reddit.com/grants/installed_client" },
                { "device_id", _parameters.DeviceId.ToString() },
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
            /// Gets or sets device identifier.
            /// </summary>
            public Guid DeviceId { get; set; }

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
