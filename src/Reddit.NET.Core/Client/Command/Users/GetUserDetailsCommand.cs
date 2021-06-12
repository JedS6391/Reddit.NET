using System;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Users
{
    public sealed class GetUserDetailsCommand : ClientCommand
    {
        public GetUserDetailsCommand()
            : base()
        {
        }

        public override string Id => nameof(GetUserDetailsCommand);

        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://oauth.reddit.com/api/v1/me")
            };

            return request;
        }
    }
}