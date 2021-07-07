using System;

namespace Reddit.NET.Client.Models.Public.Read
{
    /// <summary>
    /// Defines a read-only view of user content.
    /// </summary>
    public abstract class UserContentDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserContentDetails" /> class.
        /// </summary>
        /// <param name="kind">The kind of the thing.</param>
        /// <param name="id">The identifier of the thing.</param>
        protected UserContentDetails(string kind, string id)
        {
            Kind = kind;
            Id = id;
            FullName = $"{kind}_{id}";
        }

        /// <summary>
        /// Gets the author of the content.
        /// </summary>
        public string Author { get; protected set; }

        /// <summary>
        /// Gets the number of upvotes on the content.
        /// </summary>
        public int Upvotes { get; protected set; }

        /// <summary>
        /// Gets the number of downvotes on the content.
        /// </summary>
        public int Downvotes { get; protected set; }

        /// <summary>
        /// Gets the direction of the vote on the submission.
        /// </summary>
        public VoteDirection Vote { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the content has been saved.
        /// </summary>
        public bool Saved { get; protected set; }

        /// <summary>
        /// Gets the date and time when the content was created.
        /// </summary>
        public DateTimeOffset CreatedAtUtc { get; protected set; }

        /// <summary>
        /// Gets the kind of the content.
        /// </summary>
        /// <see href="https://www.reddit.com/dev/api/oauth/" />
        protected internal string Kind { get; protected set; }

        /// <summary>
        /// Gets the identifier of the content.
        /// </summary>
        /// <see href="https://www.reddit.com/dev/api/oauth/" />
        protected internal string Id { get; protected set; }

        /// <summary>
        /// Gets the full name of the content.
        /// </summary>
        /// <see href="https://www.reddit.com/dev/api/oauth/#fullnames" />
        protected internal string FullName { get; protected set; }
    }
}
