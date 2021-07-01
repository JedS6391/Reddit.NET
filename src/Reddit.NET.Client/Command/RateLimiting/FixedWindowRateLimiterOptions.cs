using System;

namespace Reddit.NET.Client.Command.RateLimiting
{
    /// <summary>
    /// Defines the options for <see cref="FixedWindowRateLimiter" />.
    /// </summary>
    internal class FixedWindowRateLimiterOptions
    {      
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedWindowRateLimiterOptions" /> class.
        /// </summary>
        /// <param name="permitLimit"></param>
        /// <param name="queueLimit"></param>
        /// <param name="window"></param>
        public FixedWindowRateLimiterOptions(int permitLimit, int queueLimit, TimeSpan window)
        {
            PermitLimit = permitLimit;
            QueueLimit = queueLimit;
            Window = window;
        }

        /// <summary>
        /// Gets the maximum number of permits the limiter will lease.
        /// </summary>
        public int PermitLimit { get; }

        /// <summary>
        /// Gets the maximum number of permit requests that will be queued while waiting for permits to become available.
        /// </summary>
        public int QueueLimit { get; }
        
        /// <summary>
        /// Gets the duration of the window in which permits will be obtained.
        /// </summary>
        /// <remarks>
        /// The limiter will automatically replenish its permits based on the window interval.
        /// </remarks>
        public TimeSpan Window { get; }

        /// <summary>
        /// Gets a <see cref="FixedWindowRateLimiterOptions" /> configured with default values.
        /// </summary>
        public static FixedWindowRateLimiterOptions Default => new FixedWindowRateLimiterOptions(
            permitLimit: 60,
            queueLimit: 10,
            window: TimeSpan.FromSeconds(60));
    }
}