using System;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to remove another reddit user as a friend of the authenticated user.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class RemoveFriendCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveFriendCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public RemoveFriendCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(RemoveFriendCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(RedditApiUrl.Me.Friend(_parameters.Username)),
                Content = System.Net.Http.Json.JsonContent.Create(new object())
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets the name of the user to unfriend.
            /// </summary>
            public string Username { get; set; }
        }
    }
}
