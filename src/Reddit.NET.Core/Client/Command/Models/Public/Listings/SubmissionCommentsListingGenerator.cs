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
        private readonly CommandFactory _commandFactory;
        private readonly IAuthenticator _authenticator;
        private readonly SubmissionCommentsListingGenerator.ListingParameters _parameters;

        public SubmissionCommentsListingGenerator(
            CommandFactory commandFactory, 
            IAuthenticator authenticator,
            SubmissionCommentsListingGenerator.ListingParameters parameters)
        {
            _commandFactory = commandFactory;
            _authenticator = authenticator;
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

        private async Task<Comment.Listing> GetListingAsync(string after = null)
        {
            var authenticationContext = await _authenticator.GetAuthenticationContextAsync().ConfigureAwait(false);

            var getSubmissionCommentsCommand = _commandFactory.Create<GetSubmissionCommentsCommand>();

            var result = await getSubmissionCommentsCommand
                .ExecuteAsync(authenticationContext, new GetSubmissionCommentsCommand.Parameters()
                {
                    SubredditName = _parameters.SubredditName,
                    SubmissionId = _parameters.SubmissionId
                })
                .ConfigureAwait(false);

            return result.Listing;
        }

        internal override CommentDetails MapThing(Thing<Comment.Details> thing) => new CommentDetails()
        {
            Body = thing.Data.Body           
        };

        public class ListingParameters 
        {
            public string SubredditName { get; set; }
            public string SubmissionId { get; set; }
        }
    }
}