using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Submissions
{
    /// <summary>
    /// Defines a command to get the details of a submission and its comments.
    /// </summary>
    [ReadOnlyAuthenticationContext]
    [UserAuthenticationContext]
    public sealed class GetSubmissionDetailsWithCommentsCommand : ClientCommand
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

            var queryString = BuildQueryString();

            uriBuilder.Query = $"?{queryString}";

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri
            };

            return request;
        }

        private string BuildQueryString()
        {
            var parameters = new Dictionary<string, string>()
            {
                { "limit", _parameters.Limit?.ToString(CultureInfo.InvariantCulture) },
                { "sort", _parameters.Sort },
                { "comment", _parameters.FocusCommentId }
            };

            var queryStringParameters = parameters
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .Select(p => $"{p.Key}={p.Value}");

            return string.Join('&', queryStringParameters);
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
            /// Gets or sets the identifier of the comment to focus on.
            /// </summary>
            public string FocusCommentId { get; set; }

            /// <summary>
            /// Gets or sets the sort option of the comments.
            /// </summary>
            public string Sort { get; set; }

            /// <summary>
            /// Gets or sets the limit parameter.
            /// </summary>
            public int? Limit { get; set; }
        }
    }
}
