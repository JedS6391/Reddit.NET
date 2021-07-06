using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Reddit.NET.Client.Command.UserContent
{
    /// <summary>
    /// Defines a command to reply to an inbox message.
    /// </summary>
    public sealed class ReplyToMessageCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplyToMessageCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public ReplyToMessageCommand(Parameters parameters)
            : base()            
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(ReplyToMessageCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "thing_id", _parameters.MessageFullName },
                { "text", _parameters.Text },
                // This parameter is required to ensure a proper JSON response is returned.
                { "api_type", "json" },
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.UserContent.Reply),
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
            /// Gets the full name of the message to reply to.
            /// </summary>
            public string MessageFullName { get; set; }

            /// <summary>
            /// Gets or sets the text of the reply to create.
            /// </summary>
            public string Text { get; set; }
        }
    }
}