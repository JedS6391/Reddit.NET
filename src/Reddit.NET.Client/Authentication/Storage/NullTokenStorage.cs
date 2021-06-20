using System;
using System.Threading.Tasks;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Storage
{
    /// <summary>
    /// An <see cref="ITokenStorage" /> that does not store tokens.
    /// </summary>
    public class NullTokenStorage : ITokenStorage
    {
        /// <inheritdoc />
        public Task<Token> GetTokenAsync(Guid sessionId) =>
            throw new NotSupportedException("Storage provider does not support retrieval of tokens");

        /// <inheritdoc />
        public Task StoreTokenAsync(Guid sessionId, Token token) => Task.CompletedTask;

        /// <inheritdoc />
        public Task RemoveTokenAsync(Guid sessionId) => Task.CompletedTask;
    }
}