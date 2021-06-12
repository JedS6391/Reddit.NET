using System;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Users
{
    public class GetUserSubredditsCommand : ClientCommand
    {
        private readonly GetUserSubredditsCommand.Parameters _parameters;

        public GetUserSubredditsCommand(GetUserSubredditsCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        public override string Id => nameof(GetUserSubredditsCommand);

        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder("https://oauth.reddit.com/subreddits/mine/subscriber");

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
            public string After { get; set; }
        }
    }
}