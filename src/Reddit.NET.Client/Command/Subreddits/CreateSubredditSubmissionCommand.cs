using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Subreddits
{
    /// <summary>
    /// Defines a command to create a submission in a particular subreddit.
    /// </summary>
    public sealed class CreateSubredditSubmissionCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSubredditSubmissionCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public CreateSubredditSubmissionCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(CreateSubredditSubmissionCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var requestParameters = new Dictionary<string, string>()
            {
                { "sr", _parameters.SubredditName },
                { "kind", MapSubmissionType(_parameters.Type) },
                { "title", _parameters.Title },
                { "resubmit", _parameters.ForceResubmit.ToString() },
                // These parameters are required to ensure a proper JSON response is returned.
                { "api_type", "json" },
                { "extension", "json" }
            };

            switch (_parameters.Type)
            {
                case SubmissionType.Link:
                    requestParameters["url"] = _parameters.Url;
                    break;

                case SubmissionType.Self:
                    requestParameters["text"] = _parameters.Text;
                    break;
            }

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.Subreddit.Submit),
                Content = new FormUrlEncodedContent(requestParameters)
            };

            return request;
        }

        private static string MapSubmissionType(SubmissionType type) =>
            type switch
            {
                SubmissionType.Link => "link",
                SubmissionType.Self => "self",
                _ => throw new ArgumentException($"Unsupported submission type '{type}'."),
            };

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets or sets the name of the subreddit to create the submission in.
            /// </summary>
            public string SubredditName { get; set; }

            /// <summary>
            /// Gets or sets the type of submission to create.
            /// </summary>
            public SubmissionType Type { get; set; }

            /// <summary>
            /// Gets or sets the title of the submission to create.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the URL of the submission to create.
            /// </summary>
            /// <remarks>
            /// Only applicable when <see cref="Type" /> is <see cref="SubmissionType.Link" />.
            /// </remarks>
            public string Url { get; set; }

            /// <summary>
            /// Gets or sets the text of the submission to create.
            /// </summary>
            /// <remarks>
            /// Only applicable when <see cref="Type" /> is <see cref="SubmissionType.Self" />.
            /// </remarks>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the submission should be resubmitted if it already exists.
            /// </summary>
            public bool ForceResubmit { get; set; }
        }

        /// <summary>
        /// Defines the submission types
        /// </summary>
        public enum SubmissionType
        {
            /// <summary>
            /// A submission for a link.
            /// </summary>
            Link,

            /// <summary>
            /// A submission for text (a self-post).
            /// </summary>
            Self
        }
    }
}
