using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Submissions
{
    /// <summary>
    /// Defines a command to get the duplicates of a given submission.
    /// </summary>
    [ReadOnlyAuthenticationContext]
    [UserAuthenticationContext]
    public sealed class GetDuplicateSubmissionsCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDuplicateSubmissionsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetDuplicateSubmissionsCommand(Parameters parameters)
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(GetDuplicateSubmissionsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(
                RedditApiUrl.Submission.Duplicates(_parameters.SubmissionId));

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
                { "limit", _parameters.Limit.ToString(CultureInfo.InvariantCulture) },
                { "after", _parameters.After }
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
            /// Gets or sets the sort option of the submissions.
            /// </summary>
            public string Sort { get; set; }

            /// <summary>
            /// Gets or sets the limit parameter.
            /// </summary>
            public int Limit { get; set; }

            /// <summary>
            /// Gets or sets the after parameter.
            /// </summary>
            public string After { get; set; }
        }
    }
}
