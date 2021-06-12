using System;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Submissions
{
    public class GetSubmissionCommentsCommand : ClientCommand
    {
        private readonly GetSubmissionCommentsCommand.Parameters _parameters;

        public GetSubmissionCommentsCommand(GetSubmissionCommentsCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        public override string Id => nameof(GetSubmissionCommentsCommand);

        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder($"https://oauth.reddit.com/r/{_parameters.SubredditName}/comments/{_parameters.SubmissionId}");

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
            public string SubmissionId { get; set; }
        }
    }
}