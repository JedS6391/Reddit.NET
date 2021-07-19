using System;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Multireddits
{
    /// <summary>
    /// Defines a command to delete a multireddit belonging to the currently authenticated user.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class DeleteMultiredditCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteMultiredditCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public DeleteMultiredditCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(DeleteMultiredditCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(RedditApiUrl.Multireddit.Delete(
                    _parameters.Username,
                    _parameters.MultiredditName))
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
        }
    }
}
