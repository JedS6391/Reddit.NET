using System.Threading.Tasks;

namespace Reddit.NET.Core.Client.Authentication.Abstract
{
    public interface IAuthenticator
    {
         Task<AuthenticationContext> AuthenticateAsync();
    }
}