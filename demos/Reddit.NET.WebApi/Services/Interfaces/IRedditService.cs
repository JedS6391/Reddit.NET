using System;
using System.Threading.Tasks;
using Reddit.NET.Client;

namespace Reddit.NET.WebApi.Services.Interfaces
{
    public interface IRedditService
    {
        Uri GenerateAuthorizationUri();

        Task<Guid> CompleteAuthorizationAsync(string state, string code);

        Task<RedditClient> GetClientAsync(Guid sessionId);

        Task EndSessionAsync(Guid sessionId);
    }
}