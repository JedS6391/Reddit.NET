using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Reddit.NET.Client.Command.RateLimiting
{
    /// <summary>
    /// An <see cref="IRateLimiter" /> implementation using the <see href="https://en.wikipedia.org/wiki/Token_bucket">token bucket algorithm</see>.
    ///
    /// The limiter will periodically replenish its permits to allow further permits to be obtained.
    /// </summary>
    /// <remarks>
    /// Note this implementation is based on the APIs defined in the following .NET proposal: https://github.com/dotnet/runtime/issues/52079
    ///
    /// Once this API lands in the BCL, it would be preferred to use those implementations.
    /// </remarks>
    internal class TokenBucketRateLimiter : IRateLimiter
    {
        private int _permitCount;
        private int _queueCount;

        private readonly ILogger _logger;
        private readonly TokenBucketRateLimiterOptions _options;
        private readonly Timer _renewTimer;
        private readonly object _lock = new();
        private readonly Queue<RequestRegistration> _queue = new();

        private static readonly PermitLease s_successfulLease = new RateLimitLease(true);
        private static readonly PermitLease s_failedLease = new RateLimitLease(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucketRateLimiter" /> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger" /> instance used for writing log messages.</param>
        /// <param name="options">The options for the limiter to use.</param>
        public TokenBucketRateLimiter(ILogger logger, TokenBucketRateLimiterOptions options)
        {
            _logger = logger;
            _options = options;
            _renewTimer = new Timer(
                Replenish,
                this,
                _options.ReplenishmentPeriod,
                _options.ReplenishmentPeriod);

            _permitCount = _options.PermitLimit;
            _queueCount = 0;
        }

        /// <inheritdoc />
        public int GetAvailablePermits() => _permitCount;

        /// <inheritdoc />
        public ValueTask<PermitLease> AcquireAsync(int permitCount)
        {
            _logger.LogTrace($"Attempting to acquire lease for {permitCount} permits...");

            if (permitCount < 0 || permitCount > _options.PermitLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(permitCount));
            }

            // A permit count of zero may be used to check whether there are permits.
            if (permitCount == 0 && GetAvailablePermits() > 0)
            {
                _logger.LogDebug($"Lease for {permitCount} permits successfully acquired. {_permitCount} permits remaining.");

                return ValueTask.FromResult(s_successfulLease);
            }

            if (Interlocked.Add(ref _permitCount, -permitCount) >= 0)
            {
                _logger.LogDebug($"Lease for {permitCount} permits successfully acquired. {_permitCount} permits remaining.");

                // We were able to obtain the requested number of permits.
                return ValueTask.FromResult(s_successfulLease);
            }

            // Add the permits we removed back.
            Interlocked.Add(ref _permitCount, permitCount);

            _logger.LogDebug($"Lease for {permitCount} permits not acquired.");

            if (_queueCount + permitCount > _options.QueueLimit)
            {
                _logger.LogWarning("Unable to queue request for permits as the queue is already full.");

                // The queue is already full.
                return ValueTask.FromResult(s_failedLease);
            }

            // Enqueue the request for permits.
            // The request will be completed at a later point when the count is replenished.
            var registration = new RequestRegistration(permitCount);

            _queue.Enqueue(registration);

            _logger.LogDebug($"Request for permits queued for later completion.");

            return new ValueTask<PermitLease>(registration.Source.Task);
        }

        private static void Replenish(object state)
        {
            if (state is not TokenBucketRateLimiter limiter)
            {
                return;
            }

            var logger = limiter._logger;
            var options = limiter._options;

            var availablePermits = limiter.GetAvailablePermits();
            var maxPermits = options.PermitLimit;

            if (availablePermits < maxPermits)
            {
                // Replenish the available permits.
                var permitsToAdd = Math.Min(options.TokensPerPeriod, maxPermits - availablePermits);

                logger.LogDebug($"Replenishing rate limiter [Available Permits = {availablePermits}, Permits To Add = {permitsToAdd}]");

                Interlocked.Add(ref limiter._permitCount, permitsToAdd);
            }

            ProcessPermitRequestQueue(limiter);
        }

        private static void ProcessPermitRequestQueue(TokenBucketRateLimiter limiter)
        {
            var logger = limiter._logger;
            var queue = limiter._queue;

            // Process queued requests.
            lock (limiter._lock)
            {
                while (queue.Count > 0)
                {
                    logger.LogDebug($"Processing rate limiter queue...");

                    var nextPendingRequest = queue.Peek();

                    if (Interlocked.Add(ref limiter._permitCount, -nextPendingRequest.Count) >= 0)
                    {
                        logger.LogDebug($"Lease for {nextPendingRequest.Count} permits successfully acquired.");

                        // Request can be fulfilled.
                        var request = queue.Dequeue();

                        Interlocked.Add(ref limiter._queueCount, -request.Count);

                        request.Source.SetResult(s_successfulLease);
                    }
                    else
                    {
                        logger.LogDebug($"Lease for {nextPendingRequest.Count} permits not acquired.");

                        // Request cannot be fulfilled.
                        Interlocked.Add(ref limiter._permitCount, nextPendingRequest.Count);

                        break;
                    }
                }
            }
        }

        private class RateLimitLease : PermitLease
        {
            public RateLimitLease(bool isAcquired)
            {
                IsAcquired = isAcquired;
            }

            public override bool IsAcquired { get; }

            public override void Dispose() { }
        }

        private struct RequestRegistration
        {
            public RequestRegistration(int permitCount)
            {
                Count = permitCount;
                Source = new TaskCompletionSource<PermitLease>();
            }

            public int Count { get; }

            public TaskCompletionSource<PermitLease> Source { get; }
        }
    }
}
