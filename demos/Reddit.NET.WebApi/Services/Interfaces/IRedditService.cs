using System;
using System.Threading.Tasks;
using Reddit.NET.Client;

namespace Reddit.NET.WebApi.Services.Interfaces
{
    /// <summary>
    /// Provides functionality to interact with reddit.
    /// </summary>
    public interface IRedditService
    {
        /// <summary>
        /// Creates a new authorization URI to start the OAuth login process.
        /// </summary>
        /// <returns>A <see cref="Uri" /> instance representing the authorization URI.</returns>
        Uri GenerateAuthorizationUri();

        /// <summary>
        /// Completes the authorization process initiated by a call to <see cref="GenerateAuthorizationUri" />.
        /// </summary>
        /// <param name="state">The state parameter.</param>
        /// <param name="code">The code parameter.</param>
        /// <returns>A <see cref="Guid" /> representing a session identifier for the authenticated user.</returns>
        Task<Guid> CompleteAuthorizationAsync(string state, string code);

        /// <summary>
        /// Gets a <see cref="RedditClient" /> instance for the provided session.
        /// </summary>
        /// <param name="sessionId">The session identifier obtained from a call to <see cref="CompleteAuthorizationAsync" />.</param>
        /// <returns>A <see cref="RedditClient" /> instance.</returns>
        Task<RedditClient> GetClientAsync(Guid sessionId);

        /// <summary>
        /// Ends the provided session
        /// </summary>
        /// <param name="sessionId">The session identifier obtained from a call to <see cref="CompleteAuthorizationAsync" />.</param>        
        Task EndSessionAsync(Guid sessionId);
    }
}