using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft;
using Reddit.NET.Client.Interactions;
using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a submission.
    /// </summary>
    public class SubmissionDetails : UserContentDetails, IToInteractor<SubmissionInteractor>, IReloadable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDetails" /> class.
        /// </summary>
        /// <param name="thing">A <see cref="Thing{TData}" /> containing a submission's data.</param>
        internal SubmissionDetails(IThing<Submission.Details> thing)
            : base(thing.Kind, thing.Data.Id)
        {
            Requires.NotNull(thing, nameof(thing));

            Title = thing.Data.Title;
            Subreddit = thing.Data.Subreddit;
            Permalink = thing.Data.Permalink;
            Url = thing.Data.Url;
            Domain = thing.Data.Domain;
            IsSelfPost = thing.Data.IsSelfPost;
            IsNsfw = thing.Data.IsNsfw;
            SelfText = thing.Data.SelfText;
            FlairId = thing.Data.FlairId;
            FlairText = thing.Data.FlairText;
            Author = thing.Data.Author;
            Upvotes = thing.Data.Upvotes;
            Downvotes = thing.Data.Downvotes;
            Vote = thing.Data.LikedByUser switch
            {
                true => VoteDirection.Upvoted,
                false => VoteDirection.Downvoted,
                null => VoteDirection.NoVote
            };
            Saved = thing.Data.IsSaved;
            CreatedAtUtc = thing.Data.CreatedAtUtc;
            LastLoadedAtUtc = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Gets the title of the submission.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the subreddit the submission belongs to.
        /// </summary>
        public string Subreddit { get; private set; }

        /// <summary>
        /// Gets the permalink of the submission.
        /// </summary>
        public string Permalink { get; private set; }

        /// <summary>
        /// Gets the URL of the submission
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the domain of the submission.
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the submission is a self post.
        /// </summary>
        public bool IsSelfPost { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the submission is <i>Not Safe For Work</i> (NSFW).
        /// </summary>
        public bool IsNsfw { get; private set; }

        /// <summary>
        /// Gets the raw text of the submission.
        /// </summary>
        public string SelfText { get; private set; }

        /// <summary>
        /// Gets the identifier of the flair associated with the submission.
        /// </summary>
        public string FlairId { get; set; }

        /// <summary>
        /// Gets the text of the flair associated with the submission.
        /// </summary>
        public string FlairText { get; set; }

        /// <inheritdoc />
        public DateTimeOffset LastLoadedAtUtc { get; private set; }

        /// <inheritdoc />
        public SubmissionInteractor Interact(RedditClient client) => client.Submission(Id);

        /// <inheritdoc />
        public async Task ReloadAsync(RedditClient client, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(client, nameof(client));

            var details = await client.Submission(Id).GetDetailsAsync(cancellationToken);

            Title = details.Title;
            Subreddit = details.Subreddit;
            Permalink = details.Permalink;
            Url = details.Url;
            Domain = details.Domain;
            IsSelfPost = details.IsSelfPost;
            IsNsfw = details.IsNsfw;
            SelfText = details.SelfText;
            Author = details.Author;
            Upvotes = details.Upvotes;
            Downvotes = details.Downvotes;
            Vote = details.Vote;
            Saved = details.Saved;
            CreatedAtUtc = details.CreatedAtUtc;
            LastLoadedAtUtc = DateTimeOffset.UtcNow;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Submission [Subreddit = {Subreddit}, Title = {Title}, Author = {Author}, Permalink = {Permalink}, CreatedAtUtc = {CreatedAtUtc}]";
    }
}
