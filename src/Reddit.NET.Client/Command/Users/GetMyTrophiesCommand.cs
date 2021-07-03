using System;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get the trophies of the currently authenticated user.
    /// </summary>
    public sealed class GetMyTrophiesCommand : ClientCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetMyTrophiesCommand" /> class.
        /// </summary>
        public GetMyTrophiesCommand()
            : base()
        {
        }

        /// <inheritdoc />
        public override string Id => nameof(GetMyTrophiesCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(RedditApiUrl.Me.Trophies)
            };

            return request;
        }
    }
}