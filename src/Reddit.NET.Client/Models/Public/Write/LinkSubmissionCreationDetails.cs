using System;
using Microsoft;

namespace Reddit.NET.Client.Models.Public.Write
{
    /// <summary>
    /// Represents the details to create a link submission.
    /// </summary>
    public sealed class LinkSubmissionCreationDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkSubmissionCreationDetails" /> class.
        /// </summary>
        /// <param name="title">The title of the submission to create.</param>
        /// <param name="uri">The URI of the submission to create.</param>
        /// <param name="resubmit">Whether the submission should be resubmitted if it already exists.</param>
        /// <param name="flairId">The identifier of the flair of the submission to create.</param>
        public LinkSubmissionCreationDetails(string title, Uri uri, bool resubmit = false, string flairId = null)
        {
            Requires.NotNullOrWhiteSpace(title, nameof(title));
            Requires.NotNull(uri, nameof(uri));

            Title = title;
            Uri = uri;
            Resubmit = resubmit;
            FlairId = flairId;
        }

        /// <summary>
        /// Gets the title of the submission to create.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the URI of the submission to create.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets a value indicating whether the submission should be resubmitted if it already exists.
        /// </summary>
        public bool Resubmit { get; }

        /// <summary>
        /// Gets or sets the identifier of the flair of the submission to create.
        /// </summary>
        public string FlairId { get; }
    }
}
