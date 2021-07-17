using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Storage
{
    /// <summary>
    /// An <see cref="ITokenStorage" /> that stores tokens in memory.
    /// </summary>
    /// <remarks>
    /// A static <see cref="ConcurrentDictionary{TKey, TValue}" /> is used to safely manage tokens across threads.
    /// </remarks>
    public sealed class MemoryTokenStorage : ITokenStorage
    {
        private static readonly ConcurrentDictionary<Guid, Token> s_tokens = new ConcurrentDictionary<Guid, Token>();

        /// <inheritdoc />
        public Task<Token> GetTokenAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            if (s_tokens.TryGetValue(sessionId, out var token))
            {
                return Task.FromResult(token);
            }

            return Task.FromResult<Token>(null);
        }

        /// <inheritdoc />
        public Task<Guid> StoreTokenAsync(Token token, CancellationToken cancellationToken = default)
        {
            var sessionId = Guid.NewGuid();

            s_tokens[sessionId] = token;

            return Task.FromResult(sessionId);
        }

        /// <inheritdoc />
        public Task RemoveTokenAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            s_tokens.TryRemove(sessionId, out _);

            return Task.CompletedTask;
        }
    }
}
