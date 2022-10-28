using System;
using System.Net.Http;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Subreddits
{
    /// <summary>
    /// Defines a command to get the flairs of a subreddit.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class GetSubredditFlairsCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSubredditFlairsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetSubredditFlairsCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetSubredditFlairsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(RedditApiUrl.Subreddit.Flairs(_parameters.SubredditName))
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets or sets the name of the subreddit to get flairs for.
            /// </summary>
            public string SubredditName { get; set; }
        }
    }
}
