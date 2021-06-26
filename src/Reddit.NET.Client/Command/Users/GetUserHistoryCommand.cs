using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace Reddit.NET.Client.Command.Users
{
    /// <summary>
    /// Defines a command to get history of a user.
    /// </summary>
    public class GetUserHistoryCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetUserHistoryCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public GetUserHistoryCommand(Parameters parameters)
            : base()
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(GetUserHistoryCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(
                RedditApiUrl.User.History(_parameters.Username, _parameters.HistoryType));

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
                { "sort", _parameters.Sort },
                { "t", _parameters.TimeRange },
                { "limit", _parameters.Limit.ToString(CultureInfo.InvariantCulture) },                
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
            /// Gets or sets the name of the user.
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets the type of history.
            /// </summary>
            public string HistoryType { get; set; }

            /// <summary>
            /// Gets or sets the sort option of history.
            /// </summary>
            public string Sort { get; set; }    

            /// <summary>
            /// Gets or sets the option for the time range of history.
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