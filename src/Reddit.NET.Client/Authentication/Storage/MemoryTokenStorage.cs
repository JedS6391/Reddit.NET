using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Storage
{
    public class MemoryTokenStorage : ITokenStorage
    {
        private static readonly ConcurrentDictionary<Guid, Token> _tokens = new ConcurrentDictionary<Guid, Token>();

        public Task<Token> GetTokenAsync(Guid sessionId)
        {
            return Task.FromResult(_tokens[sessionId]);
        }

        public Task StoreTokenAsync(Guid sessionId, Token token)
        {
            _tokens[sessionId] = token;

            return Task.CompletedTask;
        }

        public Task RemoveTokenAsync(Guid sessionId)
        {
            _tokens.TryRemove(sessionId, out _);

            return Task.CompletedTask;
        }
    }
}