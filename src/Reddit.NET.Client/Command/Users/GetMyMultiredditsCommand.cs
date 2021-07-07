using System;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get the multireddits of the currently authenticated user.
    /// </summary>
    public sealed class GetMyMultiredditsCommand : ClientCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetMyMultiredditsCommand" /> class.
        /// </summary>
        public GetMyMultiredditsCommand()
            : base()
        {
        }

        /// <inheritdoc />
        public override string Id => nameof(GetMyMultiredditsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(RedditApiUrl.Me.Multireddits)
            };

            return request;
        }
    }
}
