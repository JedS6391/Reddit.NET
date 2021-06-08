using Reddit.NET.Core.Client.Interactions;

namespace Reddit.NET.Core.Client
{
    /// <summary>
    /// Provides mechanisms for interacting with <see href="https://www.reddit.com">reddit</see>. 
    /// </summary>
    /// <remarks>
    /// <see cref="RedditClient" /> cannot be directly instantiated and should instead be created via the <see cref="Builder.RedditClientBuilder" /> class.
    /// </remarks>
    public sealed partial class RedditClient
    {
        /// <summary>
        /// Gets an interactor for operations relating to a specific submission.
        /// </summary>
        /// <param name="type">The type of the submission to interact with.</param>
        /// <param name="id">The ID of the submission to interact with.</param>
                /// <returns>A <see cref="SubmissionInteractor" /> instance that provides mechanisms for interacting with the requested submission.</returns>
        internal SubmissionInteractor Submission(string type, string id) => 
            new SubmissionInteractor(_commandFactory, _authenticator, type, id);
    }
}