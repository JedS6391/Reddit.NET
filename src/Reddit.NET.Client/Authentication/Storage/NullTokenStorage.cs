using System;
using System.Threading.Tasks;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Storage
{
    /// <summary>
    /// An <see cref="ITokenStorage" /> that does not store tokens.
    /// </summary>
    public sealed class NullTokenStorage : ITokenStorage
    {
        /// <inheritdoc />
        public Task<Token> GetTokenAsync(Guid sessionId) => Task.FromResult<Token>(null);

        /// <inheritdoc />
        public Task<Guid> StoreTokenAsync(Token token) => Task.FromResult(Guid.Empty);

        /// <inheritdoc />
        public Task RemoveTokenAsync(Guid sessionId) => Task.CompletedTask;
    }
}