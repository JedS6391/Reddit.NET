using System;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get the messages in the currently authenticated user's inbox.
    /// </summary>
    public sealed class GetMyInboxMessagesCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMyInboxMessagesCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetMyInboxMessagesCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetMyInboxMessagesCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(RedditApiUrl.Me.Inbox(_parameters.MessageType))
            {
                Query = $"?limit={_parameters.Limit}"
            };

            if (!string.IsNullOrEmpty(_parameters.After))
            {
                uriBuilder.Query = $"{uriBuilder.Query}&after={_parameters.After}";
            }

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets or sets message type parameter.
            /// </summary>
            public string MessageType { get; set; }

            /// <summary>
            /// Gets or sets the limit parameter.
            /// </summary>
            public int Limit { get; set; }

            /// <summary>
            /// Gets or sets the after parameter.
            /// </summary>
            public string After { get; set; }
        }
    }
}
