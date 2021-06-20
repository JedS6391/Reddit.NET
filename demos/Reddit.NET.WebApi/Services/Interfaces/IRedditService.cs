using System;
using System.Threading.Tasks;

namespace Reddit.NET.WebApi.Services.Interfaces
{
    public interface IRedditService
    {
        Uri GenerateAuthorizationUri();

        Task CompleteAuthorizationAsync(string state, string code);
    }
}