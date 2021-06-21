using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Interactions;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a submission.
    /// </summary>
    public class SubmissionDetails : UserContentDetails, IToInteractor<SubmissionInteractor>
    {   
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDetails" /> class.
        /// </summary>
        /// <param name="thing">A <see cref="Thing{TData}" /> containg a submission's data.</param>
        internal SubmissionDetails(IThing<Submission.Details> thing)
            : base(thing.Kind, thing.Data.Id)
        {             
            Title = thing.Data.Title;
            Subreddit = thing.Data.Subreddit;
            Permalink = thing.Data.Permalink;
            Url = thing.Data.Url; 
            Domain = thing.Data.Domain;
            IsSelfPost = thing.Data.IsSelfPost;
            IsNsfw = thing.Data.IsNsfw;
            SelfText = thing.Data.SelfText;
            Author = thing.Data.Author;
            Upvotes = thing.Data.Upvotes;
            Downvotes = thing.Data.Downvotes;
            CreatedAtUtc = thing.Data.CreatedAtUtc;
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
        /// Gets the URL of the submission
        /// </summary>
        public string Url { get;}

        /// <summary>
        /// Gets the domain of the submission.
        /// </summary>
        public string Domain { get; }

        /// <summary>
        /// Gets a value indicating whether the submission is a self post.
        /// </summary>        
        public bool IsSelfPost { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the submission is 'Not Safe For Work' (NSFW).
        /// </summary>
        public bool IsNsfw { get; private set; }

        /// <summary>
        /// Gets the raw text of the submission.
        /// </summary>
        public string SelfText { get; private set; }

        /// <inheritdoc />
        public SubmissionInteractor Interact(RedditClient client) => client.Submission(Id);

        /// <inheritdoc />
        public override string ToString() => 
            $"Submission [Subreddit = {Subreddit}, Title = {Title}, Author = {Author}, Permalink = {Permalink}, CreatedAtUtc = {CreatedAtUtc}]";
    }
}