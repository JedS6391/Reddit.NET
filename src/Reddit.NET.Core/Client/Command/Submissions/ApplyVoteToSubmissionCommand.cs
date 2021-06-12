using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Submissions
{
    public class ApplyVoteToSubmissionCommand : ClientCommand
    {
        private readonly ApplyVoteToSubmissionCommand.Parameters _parameters;

        public ApplyVoteToSubmissionCommand(ApplyVoteToSubmissionCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        public override string Id => nameof(ApplyVoteToSubmissionCommand);

        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "dir", ((int) _parameters.Direction).ToString(CultureInfo.InvariantCulture) },
                { "id", _parameters.Id }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://oauth.reddit.com/api/vote/"),
                Content = new FormUrlEncodedContent(requestParameters)
            };

            return request;
        }

        public class Parameters 
        {
            public string Id { get; set; }
            public VoteDirection Direction { get; set; }
        }

        public enum VoteDirection
        {
            Downvote = -1,
            Unvote = 0,    
            Upvote = 1
        }
    }
}