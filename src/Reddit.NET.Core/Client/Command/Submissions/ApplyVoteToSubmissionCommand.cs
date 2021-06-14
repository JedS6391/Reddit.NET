using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;

namespace Reddit.NET.Core.Client.Command.Submissions
{
    /// <summary>
    /// Defines a command to apply a vote to a submission.
    /// </summary>
    public class ApplyVoteToSubmissionCommand : ClientCommand
    {
        private readonly ApplyVoteToSubmissionCommand.Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyVoteToSubmissionCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public ApplyVoteToSubmissionCommand(ApplyVoteToSubmissionCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }
        
        /// <inheritdoc />
        public override string Id => nameof(ApplyVoteToSubmissionCommand);

        /// <inheritdoc />
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
                RequestUri = new Uri(RedditApiUrl.Submission.Vote),
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
            /// Gets or sets the identifier of the submission to vote on.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the direction of the vote.
            /// </summary>
            public VoteDirection Direction { get; set; }
        }

        /// <summary>
        /// Defines the voting directions.
        /// </summary>
        public enum VoteDirection
        {
            /// <summary>
            /// Apply a downvote.
            /// </summary>
            Downvote = -1,

            /// <summary>
            /// Remove an existing vote.
            /// </summary>
            Unvote = 0,    

            /// <summary>
            /// Apply an upvote.
            /// </summary>
            Upvote = 1
        }
    }
}