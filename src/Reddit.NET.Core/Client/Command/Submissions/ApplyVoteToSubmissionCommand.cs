using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Command.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Command.Submissions
{
    public class ApplyVoteToSubmissionCommand 
        : AuthenticatedCommand<ApplyVoteToSubmissionCommand.Parameters, ApplyVoteToSubmissionCommand.Result, Subreddit>
    {
        public ApplyVoteToSubmissionCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {
        }

        public override string Id => nameof(ApplyVoteToSubmissionCommand);

        protected override HttpRequestMessage BuildRequest(Parameters parameters)
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "dir", ((int) parameters.Direction).ToString() },
                { "id", parameters.Id }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://oauth.reddit.com/api/vote/"),
                Content = new FormUrlEncodedContent(requestParameters)
            };

            return request;
        }

        protected override Result MapToResult(Subreddit response) => new Result();

        public class Parameters 
        {
            public string Id { get; set; }
            public VoteDirection Direction { get; set; }
        }

        public class Result 
        {
        }

        public enum VoteDirection
        {
            Downvote = -1,
            Unvote = 0,    
            Upvote = 1
        }
    }
}