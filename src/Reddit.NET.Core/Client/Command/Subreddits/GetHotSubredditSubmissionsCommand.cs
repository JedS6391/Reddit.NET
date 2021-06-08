using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Command.Subreddits
{
    public class GetHotSubredditSubmissionsCommand 
        : AuthenticatedCommand<GetHotSubredditSubmissionsCommand.Parameters, GetHotSubredditSubmissionsCommand.Result, Submission.Listing>
    {
        public GetHotSubredditSubmissionsCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {
        }

        public override string Id => nameof(GetHotSubredditSubmissionsCommand);

        protected override HttpRequestMessage BuildRequest(Parameters parameters)
        {
            var uriBuilder = new UriBuilder($"https://oauth.reddit.com/r/{parameters.SubredditName}/hot");

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

        protected override Result MapResponse(Submission.Listing response) => new Result()
        {
            Listing = response
        };

        public class Parameters 
        {
            public string SubredditName { get; set; }
            public string After { get; set; }
        }

        public class Result 
        {
            public Submission.Listing Listing { get; set; }
        }
    }
}