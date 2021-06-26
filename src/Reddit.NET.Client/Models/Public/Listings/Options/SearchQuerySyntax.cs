namespace Reddit.NET.Client.Models.Public.Listings.Options
{
        /// <summary>
        /// Represents an option for a search query syntax.
        /// </summary>
        public sealed class SearchQuerySyntax : NamedOption
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SearchQuerySyntax" /> class.
            /// </summary>
            /// <param name="name">The name of the syntax.</param>
            private SearchQuerySyntax(string name)
                : base(name)
            { 
            }

            /// <summary>
            /// Gets the 'lucene' syntax.
            /// </summary>
            public static SearchQuerySyntax Lucene = new SearchQuerySyntax("lucene");

            /// <summary>
            /// Gets the 'cloudsearch' syntax.
            /// </summary>
            public static SearchQuerySyntax CloudSearch = new SearchQuerySyntax("cloudsearch");

            /// <summary>
            /// Gets the 'plain' syntax.
            /// </summary>            
            public static SearchQuerySyntax Plain = new SearchQuerySyntax("plain");
        }
}