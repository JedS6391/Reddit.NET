using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Submissions
{
    /// <summary>
    /// Defines a command to get more comments for a submission comment thread.
    /// </summary>
    [ReadOnlyAuthenticationContext]
    [UserAuthenticationContext]
    public sealed class LoadMoreCommentsCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadMoreCommentsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public LoadMoreCommentsCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(LoadMoreCommentsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "link_id", _parameters.SubmissionId },
                { "children", string.Join(',', _parameters.CommentIds) },
                { "sort", _parameters.Sort },
                { "api_type", "json" }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.Submission.MoreComments),
                Content = new FormUrlEncodedContent(requestParameters)
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
            /// Gets or sets the identifiers of the comment threads to load.
            /// </summary>
            public string[] CommentIds { get; set; }

            /// <summary>
            /// Gets or sets the sort option of the comments.
            /// </summary>
            public string Sort { get; set; }
        }
    }
}
