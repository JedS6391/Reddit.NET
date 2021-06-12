using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    public abstract class SubredditSubmissionsListingGenerator 
        : ListingGenerator<Submission.Listing, Submission.Details, SubmissionDetails>
    {
        protected RedditClient Client { get; }
        protected SubredditSubmissionsListingGenerator.ListingParameters Parameters { get; }

        protected SubredditSubmissionsListingGenerator(
            RedditClient client,
            SubredditSubmissionsListingGenerator.ListingParameters parameters)
        {
            Client = client;
            Parameters = parameters;
        }

        internal abstract Task<Submission.Listing> GetListingAsync(string after = null);

        internal async override Task<Submission.Listing> GetInitialListingAsync() => await GetListingAsync().ConfigureAwait(false);

        internal async override Task<Submission.Listing> GetNextListingAsync(Submission.Listing currentListing)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(currentListing.Data.After).ConfigureAwait(false);
        }

        internal override SubmissionDetails MapThing(Thing<Submission.Details> thing) => new SubmissionDetails(thing);

        public class ListingParameters 
        {
            public string SubredditName { get; set; }
        }
    }
}