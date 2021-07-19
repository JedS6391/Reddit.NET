using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to create a multireddit belonging to the currently authenticated user.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class CreateMultiredditCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMultiredditCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public CreateMultiredditCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(CreateMultiredditCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var model = new CreateMultiredditModel()
            {
                Name = _parameters.Name,
                Subreddits = _parameters
                    .Subreddits
                    .Select(s => new SubredditNameModel()
                    {
                        Name = s
                    })
                    .ToArray()
            };

            var requestParameters = new Dictionary<string, string>()
            {
                { "model", JsonSerializer.Serialize(model) },
            };

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(RedditApiUrl.Me.CreateMultireddit),
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
            /// Gets or sets the name of the multireddit to create.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the subreddit names to add to the multireddit.
            /// </summary>
            public IReadOnlyList<string> Subreddits { get; set; }
        }

        private class CreateMultiredditModel
        {
            [JsonPropertyName("display_name")]
            public string Name { get; set; }

            [JsonPropertyName("subreddits")]
            public SubredditNameModel[] Subreddits { get; set; }
        }

        private class SubredditNameModel
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
        }
    }
}
