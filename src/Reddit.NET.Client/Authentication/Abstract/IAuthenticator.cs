using System.Threading;
using System.Threading.Tasks;

namespace Reddit.NET.Client.Authentication.Abstract
{
    /// <summary>
    /// Provides mechanisms to facilitate authenticated interactions.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Gets an authentication context which can be used to perform operations that require authentication.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous operation.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the authentication context.</returns>
        Task<AuthenticationContext> GetAuthenticationContextAsync(CancellationToken cancellationToken = default);
    }
}
