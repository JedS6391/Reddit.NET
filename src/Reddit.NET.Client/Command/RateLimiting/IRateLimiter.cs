using System.Threading.Tasks;

namespace Reddit.NET.Client.Command.RateLimiting
{
    /// <summary>
    /// Provides the ability to permit access to rate limited resources.
    /// </summary>
    internal interface IRateLimiter
    {
        /// <summary>
        /// Gets the number of available permits.
        /// </summary>
        /// <returns>The number of available permits.</returns>
        int GetAvailablePermits();

        /// <summary>
        /// Acquires a lease for the given number of permits.
        /// </summary>
        /// <param name="permitCount">The number permits to acquire.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}" /> representing the asynchronous operation.
        ///
        /// The result will contain the lease obtained for the requested number of permits.
        /// </returns>
        /// <remarks>
        /// This method will wait until the requested number of permits is available or permits can no longer be acquired.
        /// </remarks>
        ValueTask<PermitLease> AcquireAsync(int permitCount);
    }
}
