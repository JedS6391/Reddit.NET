using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Command.Users
{
    public class GetUserSubredditsCommand : AuthenticatedCommand<GetUserSubredditsCommand.Parameters, GetUserSubredditsCommand.Result, Subreddit.Listing>
    {
        public GetUserSubredditsCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {
        }

        public override string Id => nameof(GetUserSubredditsCommand);

        protected override HttpRequestMessage BuildRequest(Parameters parameters)
        {
            var uriBuilder = new UriBuilder("https://oauth.reddit.com/subreddits/mine/subscriber");

            if (!string.IsNullOrEmpty(parameters.After))
            {
                uriBuilder.Query = $"?after={parameters.After}";
            }

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri
            };

            return request;
        }

        protected override Result MapToResult(Subreddit.Listing response) => new Result()
        {
            Listing = response
        };

        public class Parameters 
        {
            public string After { get; set; }
        }

        public class Result 
        {
            public Subreddit.Listing Listing { get; set; }
        }
    }
}