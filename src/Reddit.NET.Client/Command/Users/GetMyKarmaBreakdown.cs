using System;
using System.Net.Http;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get a breakdown of karma earned for the currently authenticated user.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class GetMyKarmaBreakdownCommand : ClientCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetMyKarmaBreakdownCommand" /> class.
        /// </summary>
        public GetMyKarmaBreakdownCommand()
            : base()
        {
        }

        /// <inheritdoc />
        public override string Id => nameof(GetMyKarmaBreakdownCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(RedditApiUrl.Me.KarmaBreakdown)
            };

            return request;
        }
    }
}
