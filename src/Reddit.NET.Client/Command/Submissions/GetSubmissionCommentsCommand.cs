using System;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Submissions
{
    /// <summary>
    /// Defines a command to get comments on a submission..
    /// </summary>
    public class GetSubmissionCommentsCommand : ClientCommand
    {
        private readonly GetSubmissionCommentsCommand.Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSubmissionCommentsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetSubmissionCommentsCommand(GetSubmissionCommentsCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetSubmissionCommentsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(
                RedditApiUrl.Submission.Comments(_parameters.SubredditName, _parameters.SubmissionId));

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters 
        {
            /// <summary>
            /// Gets or sets the name of the subreddit the submission is in.
            /// </summary>
            public string SubredditName { get; set; }

            /// <summary>
            /// Gets or sets the identifier of the submission.
            /// </summary>
            public string SubmissionId { get; set; }
        }
    }
}