using System;
using System.Threading.Tasks;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Storage
{
    public class NullTokenStorage : ITokenStorage
    {
        public Task<Token> GetTokenAsync(Guid sessionId) =>
            throw new NotSupportedException("Storage provider does not support retrieval of tokens");

        public Task StoreTokenAsync(Guid sessionId, Token token) => Task.CompletedTask;

        public Task RemoveTokenAsync(Guid sessionId) => Task.CompletedTask;
    }
}