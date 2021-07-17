using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Multireddits
{
    /// <summary>
    /// Defines a command to add a subreddit to a multireddit of the currently authenticated user.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class AddSubredditToMultiredditCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddSubredditToMultiredditCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public AddSubredditToMultiredditCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(AddSubredditToMultiredditCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            // The reddit API requires us to send a JSON object as a URL-encoded form content parameter.
            // I'm not sure why it requires this when the subreddit to add is also in the URL, but it is what it is...
            var model = new SubredditNameModel()
            {
                Name = _parameters.SubredditName
            };

            var requestParameters = new Dictionary<string, string>()
            {
                { "model", JsonSerializer.Serialize(model) },
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(RedditApiUrl.Multireddit.UpdateSubreddits(
                    _parameters.Username,
                    _parameters.MultiredditName,
                    _parameters.SubredditName)),
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

        private class SubredditNameModel
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    }
}
