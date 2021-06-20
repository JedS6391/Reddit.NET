using System;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Models.Public.Listings.Sorting;
using Reddit.NET.Client.Models.Public.ReadOnly;
using Reddit.NET.Client.Command.Users;

namespace Reddit.NET.Client.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the history of a user. 
    /// </summary>
    public sealed class UserHistoryListingEnumerable
        : ListingEnumerable<Listing<IUserContent>, IUserContent, UserContentDetails, UserHistoryListingEnumerable.Options>
    {
        private readonly RedditClient _client;
        private readonly UserHistoryListingEnumerable.ListingParameters _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserHistoryListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="options">The options available to the listing.</param>
        /// <param name="parameters">Parameters used when loading the listing data.</param>
        public UserHistoryListingEnumerable(
            RedditClient client, 
            UserHistoryListingEnumerable.Options options,
            UserHistoryListingEnumerable.ListingParameters parameters)
            : base(options)
        {
            _client = client;
            _parameters = parameters;
        }

        /// <inheritdoc />
        internal async override Task<Listing<IUserContent>> GetInitialListingAsync() => await GetListingAsync().ConfigureAwait(false);

        /// <inheritdoc />
        internal async override Task<Listing<IUserContent>> GetNextListingAsync(Listing<IUserContent> currentListing)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(currentListing.Data.After).ConfigureAwait(false);
        }

        /// <inheritdoc />
        internal override UserContentDetails MapThing(IThing<IUserContent> thing) => thing switch
        {
            Comment comment => new CommentDetails(comment),
            Submission submission => new SubmissionDetails(submission),
            _ => throw new ArgumentException("Unsupported thing type for user history.")
        };

        private async Task<Listing<IUserContent>> GetListingAsync(string after = null)
        {
            var commandParameters = new GetUserHistoryCommand.Parameters()
            {                
                HistoryType = ListingOptions.Sort.Name,
                After = after,
                Limit = ListingOptions.ItemsPerRequest,            
            };

            if (_parameters.UseAuthenticatedUser)
            {
                // Resolve the username for the currently authenticated user.
                var getMyDetailsCommand = new GetMyDetailsCommand();

                var user = await _client
                    .ExecuteCommandAsync<User.Details>(getMyDetailsCommand)
                    .ConfigureAwait(false);

                commandParameters.Username = user.Name;
            }
            else 
            {
                commandParameters.Username = _parameters.Username;
            }

            var getUserHistoryCommand = new GetUserHistoryCommand(commandParameters);

            var history = await _client
                .ExecuteCommandAsync<Listing<IUserContent>>(getUserHistoryCommand)
                .ConfigureAwait(false);

            return history;
        } 

        /// <summary>
        /// Defines parameters used when loading the listing data.
        /// </summary>
        public class ListingParameters 
        {
            /// <summary>
            /// Gets or sets the name of the subreddit the submission belongs to
            /// </summary>
            public string Username { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the authenticated user should be used as the target of the command.
            /// </summary>
            /// <remarks>
            /// When set to <see langword="true" />, the <see cref="Username" /> property does not need to be set.
            /// </remarks>
            public bool UseAuthenticatedUser { get; set; }
        }

        /// <summary>
        /// Defines the options available for <see cref="UserHistoryListingEnumerable" />.
        /// </summary>
        public class Options : ListingEnumerableOptions
        {
            /// <summary>
            /// Gets the option to use for sorting history.
            /// </summary>
            /// <remarks>Defaults to hot.</remarks>
            internal UserHistorySort Sort { get; set; } = UserHistorySort.Overview;

            /// <summary>
            /// Provides the ability to create <see cref="UserHistoryListingEnumerable.Options" /> instances.
            /// </summary>
            public class Builder : ListingEnumerableOptionsBuilder<Options, Builder>
            {
                /// <inheritdoc />
                protected override Builder Instance => this;

                /// <summary>
                /// Sets the sort option.
                /// </summary>
                /// <param name="sort">The option to use for sorting history.</param>
                /// <returns>The updated builder.</returns>
                public Builder WithSort(UserHistorySort sort)
                {
                    Options.Sort = sort;

                    return this;
                }          
            }
        }
    }
}