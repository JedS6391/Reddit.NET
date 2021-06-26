using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Subreddits
{
    /// <summary>
    /// Defines a command to get the submissions on the front page.
    /// </summary>
    public sealed class GetFrontPageSubmissionsCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFrontPageSubmissionsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetFrontPageSubmissionsCommand(Parameters parameters)
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetFrontPageSubmissionsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(RedditApiUrl.Subreddit.FrontPageSubmissions(_parameters.Sort));

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