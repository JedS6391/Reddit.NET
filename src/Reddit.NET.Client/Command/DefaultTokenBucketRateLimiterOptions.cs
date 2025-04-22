using System;
using System.Threading.RateLimiting;

namespace Reddit.NET.Client.Command
{
    /// <summary>
    /// Defines default options used for token bucket rate limiting.
    /// </summary>
    public static class DefaultTokenBucketRateLimiterOptions
    {
        /// <summary>
        /// Gets the default options instance.
        /// </summary>
        public static TokenBucketRateLimiterOptions Instance => new TokenBucketRateLimiterOptions()
        {
            TokenLimit = 5,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 10,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = 1
        };
    }
}
