using System.Threading.Tasks;

namespace Reddit.NET.Client.Command.RateLimiting
{
    /// <summary>
    /// Extension methods for <see cref="RateLimiter" /> instances.
    /// </summary>
    internal static class RateLimiterExtensions
    {
        /// <summary>
        /// Acquires a lease for a single permit.
        /// </summary>
        /// <param name="limiter">The <see cref="RateLimiter" /> instance to acquire the lease from.</param>
        /// <returns>
        /// A <see cref="ValueTask{TResult}" /> representing the asynchronous operation.
        ///
        /// The result will contain the lease obtained for the requested number of permits.
        /// </returns>
        public static ValueTask<PermitLease> AcquireAsync(this RateLimiter limiter) =>
            limiter.AcquireAsync(permitCount: 1);
    }
}
