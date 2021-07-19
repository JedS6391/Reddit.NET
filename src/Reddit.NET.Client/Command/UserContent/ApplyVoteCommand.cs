using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Command.UserContent
{
    /// <summary>
    /// Defines a command to apply a vote to a submission or comment.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class ApplyVoteCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyVoteCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public ApplyVoteCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(ApplyVoteCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "dir", ((int) _parameters.Direction).ToString(CultureInfo.InvariantCulture) },
                { "id", _parameters.FullName }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.UserContent.Vote),
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
            /// Gets or sets the full name of the submission or comment to vote on.
            /// </summary>
            public string FullName { get; set; }

            /// <summary>
            /// Gets or sets the direction of the vote.
            /// </summary>
            public VoteDirection Direction { get; set; }
        }
    }
}
