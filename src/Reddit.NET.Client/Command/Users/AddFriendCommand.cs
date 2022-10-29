using System;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to add another reddit user as a friend of the authenticated user.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class AddFriendCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFriendCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public AddFriendCommand(Parameters parameters)
            : base()
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(AddFriendCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
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
            /// Gets the name of the user to friend.
            /// </summary>
            public string Username { get; set; }
        }
    }
}
