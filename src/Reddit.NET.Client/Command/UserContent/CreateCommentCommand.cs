using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Reddit.NET.Client.Command.UserContent
{
    /// <summary>
    /// Defines a command to create a comment on a submission or as a reply to another comment.
    /// </summary>
    public sealed class CreateCommentCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommentCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public CreateCommentCommand(Parameters parameters)
            : base()            
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(CreateCommentCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "thing_id", _parameters.ParentFullName },
                { "text", _parameters.Text },
                // This parameter is required to ensure a proper JSON response is returned.
                { "api_type", "json" },
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.UserContent.Reply),
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
            /// Gets the full name of the parent object to add the comment on (i.e. a submission or a comment).
            /// </summary>
            public string ParentFullName { get; set; }

            /// <summary>
            /// Gets or sets the text of the comment to create.
            /// </summary>
            public string Text { get; set; }
        }
    }
}