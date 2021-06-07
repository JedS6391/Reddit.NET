using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;

namespace Reddit.NET.Core.Client.Command.Abstract
{
    public abstract class AuthenticatedCommand<TParameters, TResult, TResponse> : BaseCommand<TParameters, TResponse>
    {
        protected AuthenticatedCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {                    
        }

        protected abstract HttpRequestMessage BuildRequest(TParameters parameters);

        protected abstract TResult MapResponse(TResponse response);

        public async Task<TResult> ExecuteAsync(AuthenticationContext authenticationContext, TParameters parameters)
        {
            if (!authenticationContext.CanExecute(this))
            {
                Logger.LogError("'{CommandId}' not supported with the configured authentication scheme ('{AuthenticationContextId}').", Id, authenticationContext.Id);

                // TODO: Exception
                throw new Exception();
            }

            var request = BuildRequest(parameters);

            AddAuthorizationHeader(request, authenticationContext);

            var response = await ExecuteRequest(request).ConfigureAwait(false);

            return MapResponse(response);
        }

        private void AddAuthorizationHeader(HttpRequestMessage request, AuthenticationContext authenticationContext)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                authenticationContext.Token.AccessToken);
        }
    }
}