using Microsoft.AspNetCore.Http;
using Reddit.NET.WebApi.Services.Interfaces;

namespace Reddit.NET.WebApi.Services
{
    /// <inheritdoc />
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionService" /> class.
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public string Get(string key) =>
            _httpContextAccessor.HttpContext.Session.GetString(key);

        /// <inheritdoc />
        public void Remove(string key) =>
            _httpContextAccessor.HttpContext.Session.Remove(key);

        /// <inheritdoc />
        public void Store(string key, string value) =>
            _httpContextAccessor.HttpContext.Session.SetString(key, value);
    }
}
