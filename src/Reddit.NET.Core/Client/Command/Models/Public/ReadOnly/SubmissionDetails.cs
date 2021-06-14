using System;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDetails" /> class.
        /// </summary>
        /// <param name="thing">A <see cref="Thing{TData}" /> containg a submission's data.</param>
        internal SubmissionDetails(Thing<Submission.Details> thing)
        {            
            Title = thing.Data.Title;
            Subreddit = thing.Data.Subreddit;
            Permalink = thing.Data.Permalink;
            Upvotes = thing.Data.Upvotes;
            Downvotes = thing.Data.Downvotes;
            CreatedAtUtc = thing.Data.CreatedAtUtc;
            Id = thing.Data.Id;
            Kind = thing.Kind;
        }

        /// <summary>
        /// Gets the title of the submission.
        /// </summary>
        public string Title { get; }        

        /// <summary>
        /// Gets the subreddit the submission belongs to.
        /// </summary>
        public string Subreddit { get; }

        /// <summary>
        /// Gets the permalink of the submission.
        /// </summary>
        public string Permalink { get; }

        /// <summary>
        /// Gets the number of upvotes on the comment.
        /// </summary>
        public int Upvotes { get; }

        /// <summary>
        /// Gets the number of downvotes on the comment.
        /// </summary>
        public int Downvotes { get; }     

        /// <summary>
        /// Gets the date and time the submission was created.
        /// </summary>
        public DateTimeOffset CreatedAtUtc { get; }

        internal string Id { get; set; }     
        internal string Kind { get; set; }
        
        /// <inheritdoc />
        public SubmissionInteractor Interact(RedditClient client) => client.Submission(this);

        /// <inheritdoc />
        public override string ToString() => 
            $"Submission [Subreddit = {Subreddit}, Title = {Title}, Permalink = {Permalink}]";
    }
}