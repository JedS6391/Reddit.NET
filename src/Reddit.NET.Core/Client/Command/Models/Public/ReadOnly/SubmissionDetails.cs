using Reddit.NET.Core.Client.Command.Models.Public.Abstract;
using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client.Command.Models.Public.ReadOnly
{
    /// <summary>
    /// Defines a read-only view of a submission.
    /// </summary>
    public class SubmissionDetails : IToInteractor<SubmissionInteractor>
    {   
        public string Title { get; internal set; }        
        public string Subreddit { get; internal set; }            
        public string Permalink { get; internal set; }
        internal string Id { get; set; }     
        internal string Kind { get; set; }

        public SubmissionInteractor Interact(RedditClient client) => client.Submission(this);
    }
}