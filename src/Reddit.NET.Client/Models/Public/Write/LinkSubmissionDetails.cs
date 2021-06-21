using System;

namespace Reddit.NET.Client.Models.Public.Write
{
    /// <summary>
    /// Represents the details to create a link submission.
    /// </summary>
    public class LinkSubmissionDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkSubmissionDetails" /> class.
        /// </summary>
        /// <param name="title">The title of the submission to create.</param>
        /// <param name="uri">The URI of the submission to create.</param>
        public LinkSubmissionDetails(string title, Uri uri)
        {
            Title = title;
            Uri = uri;
        }
        
        /// <summary>
        /// Gets the title of the submission to create.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the URI of the submission to create.
        /// </summary>
        public Uri Uri { get; set; }
    }
}