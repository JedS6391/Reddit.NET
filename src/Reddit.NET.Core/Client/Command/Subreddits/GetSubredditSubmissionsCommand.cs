using System;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Subreddits
{
    /// <summary>
    /// Defines a command to get the submissions of a subreddit.
    /// </summary>
    public class GetSubredditSubmissionsCommand : ClientCommand
    {
        private readonly GetSubredditSubmissionsCommand.Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSubredditSubmissionsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetSubredditSubmissionsCommand(GetSubredditSubmissionsCommand.Parameters parameters)
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetSubredditSubmissionsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(
                RedditApiUrl.Subreddit.Submissions(_parameters.SubredditName, _parameters.Sort));

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
            /// Gets or sets the name of the subreddit to get submissions for.
            /// </summary>
            public string SubredditName { get; set; }

            /// <summary>
            /// Gets or sets the sort order of the submissions.
            /// </summary>
            public string Sort { get; set; }

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