using System;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Command.Subreddits;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    /// <summary>
    /// A <see cref="ListingEnumerable{TListing, TData, TMapped, TOptions}" /> implementation over the 'hot' submissions of a subreddit.
    /// </summary>
    public class HotSubredditSubmissionsListingEnumerable : SubredditSubmissionsListingEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotSubredditSubmissionsListingEnumerable" /> class.
        /// </summary>
        /// <param name="client">A <see cref="RedditClient" /> instance used to load the listing data.</param>
        /// <param name="parameters">Parameters used when loading the listing data.</param>
        public HotSubredditSubmissionsListingEnumerable(
            RedditClient client,
            Action<SubredditSubmissionsListingEnumerable.Options> optionsBuilder,
            SubredditSubmissionsListingEnumerable.ListingParameters parameters) 
            : base(client, optionsBuilder, parameters)
        {
        }

        /// <inheritdoc />
        internal async override Task<Submission.Listing> GetListingAsync(string after = null)
        {
            var getHotSubredditSubmissionsCommand = new GetHotSubredditSubmissionsCommand(new GetHotSubredditSubmissionsCommand.Parameters()
            {
                SubredditName = Parameters.SubredditName,
                Limit = ListingOptions.ItemsPerRequest,
                After = after
            });

            var submissions = await Client.ExecuteCommandAsync<Submission.Listing>(getHotSubredditSubmissionsCommand);

            return submissions;
        }
    }
}