using System;
using System.Threading.Tasks;
using Reddit.NET.Client.Models.Internal;

namespace Reddit.NET.Client.Authentication.Abstract
{
    public interface ITokenStorage
    {
        Task<Token> GetTokenAsync(Guid sessionId);
        Task StoreTokenAsync(Guid sessionId, Token token);
        Task RemoveTokenAsync(Guid sessionId);
    }
}