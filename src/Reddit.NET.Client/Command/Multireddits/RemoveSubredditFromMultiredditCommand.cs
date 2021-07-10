using System;
using System.Net.Http;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Multireddits
{
    /// <summary>
    /// Defines a command to remove a subreddit from a multireddit of the currently authenticated user.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class RemoveSubredditFromMultiredditCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveSubredditFromMultiredditCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public RemoveSubredditFromMultiredditCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(RemoveSubredditFromMultiredditCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(RedditApiUrl.Multireddit.UpdateSubreddits(
                    _parameters.Username,
                    _parameters.MultiredditName,
                    _parameters.SubredditName))
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets or sets the name of the user the multireddit belongs to.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the multireddit name to add the subreddit to.
            /// </summary>
            public string MultiredditName { get; set; }

            /// <summary>
            /// Gets or sets the subreddit name to add to the multireddit.
            /// </summary>
            public string SubredditName { get; set; }
        }
    }
}
