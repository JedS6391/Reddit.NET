using System;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get the friends of the currently authenticated user.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class GetMyFriendsCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMyFriendsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetMyFriendsCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(GetMyFriendsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(RedditApiUrl.Me.Friends)
            {
                Query = $"?limit={_parameters.Limit}"
            };

            if (!string.IsNullOrEmpty(_parameters.After))
            {
                uriBuilder.Query = $"{uriBuilder.Query}&after={_parameters.After}";
            }

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets or sets the limit parameter.
            /// </summary>
            public int Limit { get; set; }

            /// <summary>
            /// Gets or sets the after parameter.
            /// </summary>
            public string After { get; set; }
        }
    }
}
