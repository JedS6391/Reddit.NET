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
        public static TokenBucketRateLimiterOptions Instance => new TokenBucketRateLimiterOptions(
            tokenLimit: 5,
            queueProcessingOrder: QueueProcessingOrder.OldestFirst,
            queueLimit: 10,
            replenishmentPeriod: TimeSpan.FromSeconds(1),
            tokensPerPeriod: 1);
    }
}
