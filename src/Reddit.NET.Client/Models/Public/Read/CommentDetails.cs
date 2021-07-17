using Reddit.NET.Client.Models.Internal;
using Reddit.NET.Client.Models.Internal.Base;
using Reddit.NET.Client.Models.Public.Abstract;
using Reddit.NET.Client.Interactions;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of a comment.
    /// </summary>
    public class CommentDetails : UserContentDetails, IToInteractor<CommentInteractor>, IReloadable
    {
        internal CommentDetails(IThing<Comment.Details> thing)
            : base(thing.Kind, thing.Data.Id)
        {
            SubmissionFullName = thing.Data.LinkFullName;
            Body = thing.Data.Body;
            Subreddit = thing.Data.Subreddit;
            Permalink = thing.Data.Permalink;
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
        /// Gets the body of the comment.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Gets the subreddit the comment belongs to.
        /// </summary>
        public string Subreddit { get; private set; }

        /// <summary>
        /// Gets the permanent link of the comment.
        /// </summary>
        public string Permalink { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset LastLoadedAtUtc { get; private set; }

        /// <summary>
        /// Gets the full name of the submission the comment belongs to.
        /// </summary>
        internal string SubmissionFullName { get; private set; }

        /// <summary>
        /// Gets the identifier of the submission the comment belongs to.
        /// </summary>
        internal string SubmissionId => SubmissionFullName.Split("_")[1];

        /// <inheritdoc />
        public CommentInteractor Interact(RedditClient client) => client.Comment(SubmissionId, Id);

        /// <inheritdoc />
        public async Task ReloadAsync(RedditClient client, CancellationToken cancellationToken = default)
        {
            var details = await client.Comment(SubmissionId, Id).GetDetailsAsync(cancellationToken);

            Body = details.Body;
            Subreddit = details.Subreddit;
            Permalink = details.Permalink;
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
            $"Comment [Subreddit = {Subreddit}, Author = {Author}, Permalink = {Permalink}, CreatedAtUtc = {CreatedAtUtc}]";
    }
}
