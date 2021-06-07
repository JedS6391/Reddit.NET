using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Reddit.NET.Core.Client.Command.Abstract
{
    public abstract class UnauthenticatedCommand<TParameters, TResult, TResponse> : BaseCommand<TParameters, TResponse>
    {
        protected UnauthenticatedCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {                    
        }

        protected abstract HttpRequestMessage BuildRequest(TParameters parameters);

        protected abstract TResult MapResponse(TResponse response);

        public async Task<TResult> ExecuteAsync(TParameters parameters)
        {
            var request = BuildRequest(parameters);

            var response = await ExecuteRequest(request).ConfigureAwait(false);

            return MapResponse(response);
        }
    }
}