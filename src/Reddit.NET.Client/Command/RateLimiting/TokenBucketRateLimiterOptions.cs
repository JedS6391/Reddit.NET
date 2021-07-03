using System;

namespace Reddit.NET.Client.Command.RateLimiting
{
    /// <summary>
    /// Defines the options for <see cref="TokenBucketRateLimiterOptions" />.
    /// </summary>
    internal class TokenBucketRateLimiterOptions
    {      
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucketRateLimiterOptions" /> class.
        /// </summary>
        /// <param name="permitLimit">The maximum number of permits the limiter will lease.</param>
        /// <param name="queueLimit">The maximum number of permit requests that will be queued.</param>
        /// <param name="replenishmentPeriod">The duration of time between permit replenishments.</param>
        /// /// <param name="tokensPerPeriod">The number of permits to replenish each period.</param>
        public TokenBucketRateLimiterOptions(
            int permitLimit, 
            int queueLimit, 
            TimeSpan replenishmentPeriod,
            int tokensPerPeriod)
        {
            PermitLimit = permitLimit;
            QueueLimit = queueLimit;
            ReplenishmentPeriod = replenishmentPeriod;
            TokensPerPeriod = tokensPerPeriod;
        }

        /// <summary>
        /// Gets the maximum number of permits the limiter will lease.
        /// </summary>
        /// <remarks>
        /// Set this to a value greater than <see cref="TokensPerPeriod" /> to allow a number of permits to be leased at once outside of the standard rate.
        /// </remarks>
        public int PermitLimit { get; }

        /// <summary>
        /// Gets the maximum number of permit requests that will be queued while waiting for permits to become available.
        /// </summary>
        public int QueueLimit { get; }
        
        /// <summary>
        /// Gets the duration of time between permit replenishments.
        /// </summary>
        /// <remarks>
        /// The limiter will automatically replenish its permits based on this interval.
        /// </remarks>
        public TimeSpan ReplenishmentPeriod { get; }

        /// <summary>
        /// Gets the number of permits to replenish each period.
        /// </summary>
        public int TokensPerPeriod { get; }        

        /// <summary>
        /// Gets a <see cref="TokenBucketRateLimiterOptions" /> configured with default values.
        /// </summary>
        /// <remarks>
        /// The default options allow up to 5 permits to be obtained at once, replenishing at a rate of 1 permit per second.
        ///
        /// When no more permits are available, up to 10 permit requests can be queued for completion as the permits are replenished. 
        /// </remarks>
        public static TokenBucketRateLimiterOptions Default => new TokenBucketRateLimiterOptions(
            permitLimit: 5,
            queueLimit: 10,
            replenishmentPeriod: TimeSpan.FromSeconds(1),
            tokensPerPeriod: 1);
    }
}