using System.Net.Http;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Command.Models.Public.ReadOnly;
using Reddit.NET.Core.Client.Command.Submissions;

namespace Reddit.NET.Core.Client.Command.Models.Public.Listings
{
    public class SubmissionCommentsListingGenerator 
        : ListingGenerator<Comment.Listing, Comment.Details, CommentDetails>
    {
        private readonly RedditClient _client;
        private readonly SubmissionCommentsListingGenerator.ListingParameters _parameters;

        public SubmissionCommentsListingGenerator(
            RedditClient client,
            SubmissionCommentsListingGenerator.ListingParameters parameters)
        {
            _client = client;
            _parameters = parameters;
        }

        internal async override Task<Comment.Listing> GetInitialListingAsync() => await GetListingAsync().ConfigureAwait(false);

        internal async override Task<Comment.Listing> GetNextListingAsync(Comment.Listing currentListing)
        {
            if (string.IsNullOrEmpty(currentListing.Data.After))
            {
                return null;
            }

            return await GetListingAsync(currentListing.Data.After).ConfigureAwait(false);
        }

        internal override CommentDetails MapThing(Thing<Comment.Details> thing) => new CommentDetails(thing);

        private async Task<Comment.Listing> GetListingAsync(string after = null)
        {
            var getSubmissionCommentsCommand = new GetSubmissionCommentsCommand(new GetSubmissionCommentsCommand.Parameters()
            {
                SubredditName = _parameters.SubredditName,
                SubmissionId = _parameters.SubmissionId
            });
        
            var response = await _client.ExecuteCommandAsync(getSubmissionCommentsCommand);

            var comments = ParseResponse(response);

            return comments;
        }    

        private Comment.Listing ParseResponse(HttpResponseMessage response)
        {
            // TODO: Need a better way to handle custom response parsing.
            // TODO: Implement parsing logic
            return null;
        }

        public class ListingParameters 
        {
            public string SubredditName { get; set; }
            public string SubmissionId { get; set; }
        }
    }
}