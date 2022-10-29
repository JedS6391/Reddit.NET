using System;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.UserContent
{
    /// <summary>
    /// Defines a command to award a submission or comment.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class AwardContentCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AwardContentCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public AwardContentCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(AwardContentCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.UserContent.Award(_parameters.FullName))
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets or sets the full name of the submission or comment to award.
            /// </summary>
            public string FullName { get; set; }
        }
    }
}
