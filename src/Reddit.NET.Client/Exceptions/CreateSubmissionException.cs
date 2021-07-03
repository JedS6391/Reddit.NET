using System.Collections.Generic;
using Reddit.NET.Client.Models.Public.Read;

namespace Reddit.NET.Client.Exceptions
{
    /// <summary>
    /// Represents an error that occurs when creating a submission.
    /// </summary>
    public class CreateSubmissionException : RedditClientException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSubmissionException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="details">The details of the error.</param>
        public CreateSubmissionException(string message, ErrorDetails details)
            : base(message)
        {
            Details = details;
        }

        /// <summary>
        /// Gets the details of the error.
        /// </summary>
        public ErrorDetails Details { get; }
    }
}