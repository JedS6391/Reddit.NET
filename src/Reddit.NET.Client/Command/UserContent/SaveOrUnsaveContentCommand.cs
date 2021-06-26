using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Reddit.NET.Client.Command.UserContent
{
    /// <summary>
    /// Defines a command to save or unsave a submission or comment.
    /// </summary>
    public class SaveOrUnsaveContentCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveOrUnsaveContentCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public SaveOrUnsaveContentCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }
        
        /// <inheritdoc />
        public override string Id => nameof(SaveOrUnsaveContentCommand);

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
                // Determine which URL depending on whether we're saving or unsaving.
                RequestUri = _parameters.Unsave ?
                    new Uri(RedditApiUrl.UserContent.Unsave) :
                    new Uri(RedditApiUrl.UserContent.Save),
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
            /// Gets or sets the full name of the submission or comment to save.
            /// </summary>
            public string FullName { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the content should be saved or unsaved.
            /// </summary>
            public bool Unsave { get; set; }
        }
    }
}