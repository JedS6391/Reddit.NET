using System;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get history of a user.
    /// </summary>
    public class GetUserHistoryCommand : ClientCommand
    {
        private readonly GetUserHistoryCommand.Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserHistoryCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetUserHistoryCommand(GetUserHistoryCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetUserHistoryCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(
                RedditApiUrl.User.History(_parameters.Username, _parameters.HistoryType));

            uriBuilder.Query = $"?limit={_parameters.Limit}";

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
            /// Gets or sets the name of the user.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the type of history.
            /// </summary>
            public string HistoryType { get; set; }

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