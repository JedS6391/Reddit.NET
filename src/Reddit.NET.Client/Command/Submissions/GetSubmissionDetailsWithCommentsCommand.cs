using System;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Submissions
{
    /// <summary>
    /// Defines a command to get the details of a submission and its comments.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class GetSubmissionDetailsWithCommentsCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSubmissionDetailsWithCommentsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetSubmissionDetailsWithCommentsCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetSubmissionDetailsWithCommentsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(
                RedditApiUrl.Submission.DetailsWithComments(_parameters.SubmissionId));

            if (_parameters.Limit.HasValue)
            {
                uriBuilder.Query = $"?limit={_parameters.Limit}";
            }

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
            /// Gets or sets the identifier of the submission.
            /// </summary>
            public string SubmissionId { get; set; }

            /// <summary>
            /// Gets or sets the limit parameter.
            /// </summary>
            public int? Limit { get; set; }            
        }
    }
}