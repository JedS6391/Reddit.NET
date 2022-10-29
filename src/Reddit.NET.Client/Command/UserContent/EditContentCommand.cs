using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.UserContent
{
    /// <summary>
    /// Defines a command to edit a submission or comment.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class EditContentCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditContentCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public EditContentCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(EditContentCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "thing_id", _parameters.FullName },
                { "text", _parameters.Text }
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.UserContent.Edit),
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
            /// Gets or sets the full name of the submission or comment to edit.
            /// </summary>
            public string FullName { get; set; }

            /// <summary>
            /// Gets or sets the text to edit the submission or comment with.
            /// </summary>
            public string Text { get; set; }
        }
    }
}
