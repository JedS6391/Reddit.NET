using Reddit.NET.Core.Client.Command.Models.Internal;
using Reddit.NET.Core.Client.Command.Models.Internal.Base;
using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    /// <summary>
    /// Defines a read-only view of a submission.
    /// </summary>
    public class SubmissionDetails : IToInteractor<SubmissionInteractor>
    {   
        public SubmissionDetails(Thing<Submission.Details> submission)
        {
            Id = submission.Data.Id;
            Title = submission.Data.Title;
            Subreddit = submission.Data.Subreddit;
            Permalink = submission.Data.Permalink;
            Kind = submission.Kind;
        }

        public string Title { get; internal set; }        
        public string Subreddit { get; internal set; }            
        public string Permalink { get; internal set; }
        internal string Id { get; set; }     
        internal string Kind { get; set; }

        public SubmissionInteractor Interact(RedditClient client) => client.Submission(this);
    }
}