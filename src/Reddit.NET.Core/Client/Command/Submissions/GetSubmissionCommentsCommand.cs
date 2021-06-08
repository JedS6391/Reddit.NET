using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Command.Submissions
{
    public class GetSubmissionCommentsCommand 
        : AuthenticatedCommand<GetSubmissionCommentsCommand.Parameters, GetSubmissionCommentsCommand.Result, Comment.Listing>
    {
        public GetSubmissionCommentsCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory, ResponseMappers.SubmissionCommentsMapper)
        {
        }

        public override string Id => nameof(GetSubmissionCommentsCommand);

        protected override HttpRequestMessage BuildRequest(Parameters parameters)
        {
            var uriBuilder = new UriBuilder($"https://oauth.reddit.com/r/{parameters.SubredditName}/comments/{parameters.SubmissionId}");

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri
            };

            return request;
        }

        protected override Result MapToResult(Comment.Listing response) => new Result()
        {
            Listing = response
        };

        public class Parameters 
        {
            public string SubredditName { get; set; }
            public string SubmissionId { get; set; }
        }

        public class Result 
        {
            public Comment.Listing Listing { get; set; }
        }
    }
}