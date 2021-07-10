using System;
using System.Collections.Generic;
using System.Net.Http;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.UserContent
{
    /// <summary>
    /// Defines a command to delete a submission or comment.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class DeleteContentCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteContentCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public DeleteContentCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(DeleteContentCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "id", _parameters.FullName }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.UserContent.Delete),
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
            /// Gets or sets the full name of the submission or comment to delete.
            /// </summary>
            public string FullName { get; set; }
        }
    }
}
