using System;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get the subreddits of the currently authenticated user.
    /// </summary>
    public sealed class GetMySubredditsCommand : ClientCommand
    {
        private readonly GetMySubredditsCommand.Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMySubredditsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetMySubredditsCommand(GetMySubredditsCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetMySubredditsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(RedditApiUrl.Me.Subreddits);

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