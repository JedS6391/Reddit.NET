using System;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Abstract
{
    /// <summary>
    /// Provides mechanisms for managing the storage of tokens obtained from reddit.
    /// </summary>
    /// <remarks>
    /// Token storage is mainly applicable for contexts where an interactive authentication mode is used.
    /// 
    /// Storing the token against a particular session identifier allows the application to start performing
    /// authenticated actions without restarting the interactive authentication process.
    /// </remarks>
    public interface ITokenStorage
    {
        /// <summary>
        /// Gets the token associated with the provided session identifier.
        /// </summary>
        /// <param name="sessionId">A session identifier obtained during the interactive authentication process.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the <see cref="Token" /> associated with the provided session identifier.</returns>
        Task<Token> GetTokenAsync(Guid sessionId);

        /// <summary>
        /// Stores the provided token, associating it with the session identifier.
        /// </summary>
        /// <param name="sessionId">A session identifier obtained during the interactive authentication process.</param>
        /// <param name="token">The token to store and associate with the provided session identifier.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task StoreTokenAsync(Guid sessionId, Token token);

        /// <summary>
        /// Removes the token associated with the provided session identifier.
        /// </summary>
        /// <param name="sessionId">A session identifier obtained during the interactive authentication process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveTokenAsync(Guid sessionId);
    }
}