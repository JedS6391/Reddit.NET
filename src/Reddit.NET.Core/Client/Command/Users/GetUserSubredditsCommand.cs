using System;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get the subreddits of the currently authenticated user.
    /// </summary>
    public sealed class GetUserSubredditsCommand : ClientCommand
    {
        private readonly GetUserSubredditsCommand.Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserSubredditsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetUserSubredditsCommand(GetUserSubredditsCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetUserSubredditsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(RedditApiUrl.Me.Subreddits);

            if (!string.IsNullOrEmpty(_parameters.After))
            {
                uriBuilder.Query = $"?after={_parameters.After}";
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
            /// Gets or sets the after parameter.
            /// </summary>
            public string After { get; set; }
        }
    }
}