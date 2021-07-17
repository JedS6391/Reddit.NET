using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Microsoft;
using Reddit.NET.Client.Authentication.Context;

namespace Reddit.NET.Client.Command.Subreddits
{
    /// <summary>
    /// Defines a command to get the submissions of a multireddit.
    /// </summary>
    [UserAuthenticationContext]
    public sealed class GetMultiredditSubmissionsCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMultiredditSubmissionsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetMultiredditSubmissionsCommand(Parameters parameters)
        {
            _parameters = Requires.NotNull(parameters, nameof(parameters));
        }

        /// <inheritdoc />
        public override string Id => nameof(GetMultiredditSubmissionsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(
                RedditApiUrl.Multireddit.Submissions(
                    _parameters.Username,
                    _parameters.MultiredditName,
                    _parameters.Sort));

            var queryString = BuildQueryString();

            uriBuilder.Query = $"?{queryString}";

            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri
            };

            return request;
        }

        private string BuildQueryString()
        {
            var parameters = new Dictionary<string, string>()
            {
                { "limit", _parameters.Limit.ToString(CultureInfo.InvariantCulture) },
                { "t", _parameters.TimeRange },
                { "after", _parameters.After }
            };

            var queryStringParameters = parameters
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .Select(p => $"{p.Key}={p.Value}");

            return string.Join('&', queryStringParameters);
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
            /// Gets or sets the name of the multireddit to load submissions from.
            /// </summary>
            public string MultiredditName { get; set; }

            /// <summary>
            /// Gets or sets the sort option of the submissions.
            /// </summary>
            public string Sort { get; set; }

            /// <summary>
            /// Gets or sets the option for the time range of submissions.
            /// </summary>
            public string TimeRange { get; set; }

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
