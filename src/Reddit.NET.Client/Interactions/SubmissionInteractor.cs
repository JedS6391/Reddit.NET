using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Public.Listings;
using Reddit.NET.Client.Models.Public.Read;
using Reddit.NET.Client.Interactions.Abstract;
using Reddit.NET.Client.Command.Submissions;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Interactions
{
    /// <summary>
    /// Provides mechanisms for interacting with a submission.
    /// </summary>
    public sealed class SubmissionInteractor : UserContentInteractor, IInteractor
    {                
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionInteractor" /> class.
        /// </summary>        
        /// <param name="client">A <see cref="RedditClient" /> instance that can be used to interact with reddit.</param>
        /// <param name="submissionId">The base-36 ID of the submission to interact with.</param>
        public SubmissionInteractor(RedditClient client, string submissionId)
            : base(client, kind: Constants.Kind.Submission, id: submissionId)
        {            
        }

        /// <summary>
        /// Gets the details of the submission.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains the details of the submission.</returns>
        public async Task<SubmissionDetails> GetDetailsAsync()
        {
            var commandParameters = new GetSubmissionDetailsWithCommentsCommand.Parameters()
            {
                SubmissionId = Id,
                // Don't fetch any comments since we're just interested in the submission details
                Limit = 0
            };

            var getSubmissionDetailsWithCommentsCommand = new GetSubmissionDetailsWithCommentsCommand(commandParameters);

            var submissionWithComments = await Client
                .ExecuteCommandAsync<Submission.SubmissionWithComments>(getSubmissionDetailsWithCommentsCommand)
                .ConfigureAwait(false);

            return new SubmissionDetails(submissionWithComments.Submission);
        }

        /// <summary>
        /// Gets a <see cref="CommentThreadNavigator" /> over the comments on the submission.
        /// </summary>        
        /// <returns>
        /// A task representing the asynchronous operation. The result contains a navigator over the comments.
        /// </returns>
        public async Task<CommentThreadNavigator> GetCommentsAsync() 
        {
            var commandParameters = new GetSubmissionDetailsWithCommentsCommand.Parameters()
            {
                SubmissionId = Id
            };

            var getSubmissionDetailsWithCommentsCommand = new GetSubmissionDetailsWithCommentsCommand(commandParameters);

            var submissionWithComments = await Client
                .ExecuteCommandAsync<Submission.SubmissionWithComments>(getSubmissionDetailsWithCommentsCommand)
                .ConfigureAwait(false);

            return new CommentThreadNavigator(                
                submission: submissionWithComments.Submission,
                replies: submissionWithComments.Comments.Children);
        }  
    }
}