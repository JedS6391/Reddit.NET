using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to send a message to another reddit user.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class SendMessageCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendMessageCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public SendMessageCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(SendMessageCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "to", _parameters.Username },
                { "subject", _parameters.Subject },
                { "text", _parameters.Body },
                // This parameter is required to ensure a proper JSON response is returned.
                { "api_type", "json" },
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.Me.SendMessage),
                Content = new FormUrlEncodedContent(requestParameters)
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets the name of the user to send the message to.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the subject of the message to send.
            /// </summary>
            public string Subject { get; set; }

            /// <summary>
            /// Gets or sets the body of the message to send.
            /// </summary>
            public string Body { get; set; }
        }
    }
}
