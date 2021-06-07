using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Public;

namespace Reddit.NET.Core.Client.Command.Subreddits
{
    public class GetSubredditDetailsCommand : AuthenticatedCommand<GetSubredditDetailsCommand.Parameters, GetSubredditDetailsCommand.Result, Subreddit>
    {
        public GetSubredditDetailsCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {
        }

        public override string Id => nameof(GetSubredditDetailsCommand);

        protected override HttpRequestMessage BuildRequest(Parameters parameters)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://oauth.reddit.com/r/{parameters.SubredditName}/about")
            };

            return request;
        }

        protected override Result MapResponse(Subreddit response) => new Result()
        {
            Details = new SubredditDetails()
            {
                Name = response.Data.DisplayName,
                Title = response.Data.Title
            }
        };

        public class Parameters 
        {
            public string SubredditName { get; set; }
        }

        public class Result 
        {
            public SubredditDetails Details { get; internal set; }
        }
    }
}