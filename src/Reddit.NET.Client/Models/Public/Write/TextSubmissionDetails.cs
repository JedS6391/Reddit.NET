namespace Reddit.NET.Client.Models.Public.Write
{
    /// <summary>
    /// Represents the details to create a text submission.
    /// </summary>
    public class TextSubmissionDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextSubmissionDetails" /> class.
        /// </summary>
        /// <param name="title">The title of the submission to create.</param>
        /// <param name="text">The text of the submission to create.</param>
        public TextSubmissionDetails(string title, string text)
        {
            Title = title;
            Text = text;
        }
        
        /// <summary>
        /// Gets the title of the submission to create.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the text of the submission to create.
        /// </summary>
        public string Text { get; set; }
    }
}