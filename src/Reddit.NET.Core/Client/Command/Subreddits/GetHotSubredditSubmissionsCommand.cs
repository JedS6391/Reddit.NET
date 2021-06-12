using System;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Subreddits
{
    public class GetHotSubredditSubmissionsCommand : ClientCommand
    {
        private readonly GetHotSubredditSubmissionsCommand.Parameters _parameters;

        public GetHotSubredditSubmissionsCommand(GetHotSubredditSubmissionsCommand.Parameters parameters)
        {
            _parameters = parameters;
        }

        public override string Id => nameof(GetHotSubredditSubmissionsCommand);

        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder($"https://oauth.reddit.com/r/{_parameters.SubredditName}/hot");

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

        public class Parameters 
        {
            public string SubredditName { get; set; }
            public string After { get; set; }
        }
    }
}