using System;
using System.Net.Http;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Subreddits
{
    /// <summary>
    /// Defines a command to get the details of a subreddit.
    /// </summary>
    [ReadOnlyAuthenticationContext]
    [UserAuthenticationContext]
    public sealed class GetSubredditDetailsCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSubredditDetailsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetSubredditDetailsCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetSubredditDetailsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(RedditApiUrl.Subreddit.Details(_parameters.SubredditName))
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets or sets the name of the subreddit to get details for.
            /// </summary>
            public string SubredditName { get; set; }
        }
    }
}
