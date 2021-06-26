using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Reddit.NET.Client.Command.Subreddits
{
    /// <summary>
    /// Defines a command to search the submissions of a subreddit.
    /// </summary>
    public sealed class SearchSubredditSubmissionsCommand : ClientCommand
    {
        private readonly Parameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchSubredditSubmissionsCommand" /> class.
        /// </summary>
        /// <param name="parameters">The parameters used by the command.</param>
        public SearchSubredditSubmissionsCommand(Parameters parameters)
        {
            _parameters = parameters;
        }

        /// <inheritdoc />
        public override string Id => nameof(SearchSubredditSubmissionsCommand);

        /// <inheritdoc />
        public override HttpRequestMessage BuildRequest()
        {
            var uriBuilder = new UriBuilder(RedditApiUrl.Subreddit.Search(_parameters.SubredditName));

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
                { "q", HttpUtility.UrlEncode(_parameters.Query) },
                { "syntax", _parameters.Syntax },
                // Only search in the provided subreddit
                { "restrict_sr", bool.TrueString },                
                // Only want submission results
                { "type", "link" },   
                { "sort", _parameters.Sort },
                { "t", _parameters.TimeRange },
                { "limit", _parameters.Limit.ToString(CultureInfo.InvariantCulture) },
                { "after", _parameters.After },
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
            /// Gets or sets the name of the subreddit to search submissions in.
            /// </summary>
            public string SubredditName { get; set; }

            /// <summary>
            /// Gets or sets the query to search.
            /// </summary>
            public string Query { get; set; }

            /// <summary>
            /// Gets the search syntax used by <see cref="Query" />.
            /// </summary>
            public string Syntax { get; set; }        

            /// <summary>
            /// Gets or sets the sort option of the search.
            /// </summary>
            public string Sort { get; set; }

            /// <summary>
            /// Gets or sets the option for the time range of the search.
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