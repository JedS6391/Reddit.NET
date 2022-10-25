using Microsoft;

namespace Reddit.NET.Client.Models.Public.Write
{
    /// <summary>
    /// Represents the details to create a text submission.
    /// </summary>
    public sealed class TextSubmissionCreationDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextSubmissionCreationDetails" /> class.
        /// </summary>
        /// <param name="title">The title of the submission to create.</param>
        /// <param name="text">The text of the submission to create.</param>
        /// <param name="flairId">The identifier of the flair of the submission to create.</param>
        public TextSubmissionCreationDetails(string title, string text, string flairId = null)
        {
            Requires.NotNullOrWhiteSpace(title, nameof(title));
            Requires.NotNullOrWhiteSpace(text, nameof(text));

            Title = title;
            Text = text;
            FlairId = flairId;
        }

        /// <summary>
        /// Gets the title of the submission to create.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the text of the submission to create.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets or sets the identifier of the flair of the submission to create.
        /// </summary>
        public string FlairId { get; }
    }
}
