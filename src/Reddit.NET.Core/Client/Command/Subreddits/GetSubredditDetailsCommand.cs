using System;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Subreddits
{
    public sealed class GetSubredditDetailsCommand : ClientCommand
    {
        private readonly GetSubredditDetailsCommand.Parameters _parameters;

        public GetSubredditDetailsCommand(GetSubredditDetailsCommand.Parameters parameters)
            : base()            
        {
            _parameters = parameters;
        }

        public override string Id => nameof(GetSubredditDetailsCommand);

        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://oauth.reddit.com/r/{_parameters.SubredditName}/about")
            };

            return request;
        }

        public class Parameters 
        {
            public string SubredditName { get; set; }
        }
    }
}