using System;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get the trophies of a specific user.
    /// </summary>
    [ReadOnlyAuthenticationContext]
    [UserAuthenticationContext]
    public sealed class GetUserTrophiesCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserTrophiesCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetUserTrophiesCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(GetUserTrophiesCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(RedditApiUrl.User.Trophies(_parameters.Username))
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets the name of the user to get the trophies of.
            /// </summary>
            public string Username { get; set; }
        }
    }
}
