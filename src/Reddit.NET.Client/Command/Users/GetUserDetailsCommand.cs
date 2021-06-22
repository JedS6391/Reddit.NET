using System;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get the details of a specific user.
    /// </summary>
    public sealed class GetUserDetailsCommand : ClientCommand
    {
        private readonly GetUserDetailsCommand.Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserDetailsCommand" /> class.
        /// </summary>
        public GetUserDetailsCommand(GetUserDetailsCommand.Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetUserDetailsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(RedditApiUrl.User.Details(_parameters.Username))
            };

            return request;
        }

        /// <summary>
        /// Defines the parameters of the command.
        /// </summary>
        public class Parameters 
        {
            /// <summary>
            /// Gets or sets the name of the user.
            /// </summary>
            public string Username { get; set; }           
        }        
    }
}