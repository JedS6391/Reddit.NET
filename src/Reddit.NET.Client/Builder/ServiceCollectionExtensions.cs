using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Reddit.NET.Client.Builder
{
    /// <summary>
    /// Extension methods to configure an <see cref="IServiceCollection" /> for the client.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IHttpClientFactory" /> and related services to the <see cref="IServiceCollection" />.
        /// </summary>
        /// <remarks>
        /// A named <see cref="HttpClient" /> is defined with the appropriate configuration for the client
        /// to use for all HTTP communication.
        /// </remarks>
        /// <param name="services">The <see cref="IServiceCollection" />.</param>
        /// <param name="userAgent">The user agent the <see cref="HttpClient" /> will use.</param>
        /// <returns>The updated <see cref="IServiceCollection" />.</returns>
        public static IServiceCollection AddRedditHttpClient(this IServiceCollection services, string userAgent)
        {
            services.AddHttpClient(Constants.HttpClientName, client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json; charset=UTF-8");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
            });

            return services;
        }
    }
}
