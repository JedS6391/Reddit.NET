using System;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get the details of the currently authenticated user.
    /// </summary>
    public sealed class GetMyDetailsCommand : ClientCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetMyDetailsCommand" /> class.
        /// </summary>
        public GetMyDetailsCommand()
            : base()
        {
        }

        /// <inheritdoc />
        public override string Id => nameof(GetMyDetailsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(RedditApiUrl.Me.Details)
            };

            return request;
        }
    }
}