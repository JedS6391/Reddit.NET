using System;
using System.Collections.Concurrent;
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
    public class MemoryTokenStorage : ITokenStorage
    {
        private static readonly ConcurrentDictionary<Guid, Token> _tokens = new ConcurrentDictionary<Guid, Token>();

        /// <inheritdoc />
        public Task<Token> GetTokenAsync(Guid sessionId) => Task.FromResult(_tokens[sessionId]);        

        /// <inheritdoc />
        public Task StoreTokenAsync(Guid sessionId, Token token)
        {
            _tokens[sessionId] = token;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task RemoveTokenAsync(Guid sessionId)
        {
            _tokens.TryRemove(sessionId, out _);

            return Task.CompletedTask;
        }
    }
}