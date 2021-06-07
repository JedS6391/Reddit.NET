using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.NET.Core.Client.Authentication.Abstract;
using Reddit.NET.Core.Client.Command.Exceptions;

namespace Reddit.NET.Core.Client.Command.Abstract
{
    /// <summary>
    /// Represents a command that executes an HTTP request which requires authentication.
    /// </summary>
    /// <typeparam name="TParameters">The type of parameters accepted as input.</typeparam>
    /// <typeparam name="TResult">The type returned upon successful execution.</typeparam>
    /// <typeparam name="TResponse">The type of response to generate.</typeparam>
    public abstract class AuthenticatedCommand<TParameters, TResult, TResponse> : BaseCommand<TResponse>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticatedCommand{TParameters, TResult, TResponse}" /> class.
        /// </summary>
        /// <param name="httpClientFactory">An <see cref="IHttpClientFactory" /> instance used to create clients when executing requests</param>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory" /> instance used create a logger for writing log messages.</param>
        protected AuthenticatedCommand(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
            : base(httpClientFactory, loggerFactory)
        {                    
        }

        /// <summary>
        /// Builds an <see cref="HttpRequestMessage" /> instance from the provided parameters.
        /// </summary>
        /// <remarks>
        /// The HTTP request built will be used when the command is executed to generate the response.
        /// </remarks>
        /// <param name="parameters">The parameters of the command.</param>
        /// <returns>An HTTP request representing the command.</returns>
        protected abstract HttpRequestMessage BuildRequest(TParameters parameters);

        /// <summary>
        /// Maps the response of the HTTP request to the result type.
        /// </summary>
        /// <param name="response">The response of the HTTP request this command represents.</param>
        /// <returns>The mapped response type.</returns>
        protected abstract TResult MapResponse(TResponse response);

        /// <summary>
        /// Executes the command in the provided <see cref="AuthenticationContext" />.
        /// </summary>
        /// <remarks>
        /// The provided <see cref="AuthenticationContext" /> will be used to determine whether the command can be executed.
        /// </remarks>
        /// <param name="authenticationContext">An <see cref="AuthenticationContext" /> which controls how to authenticate the command.</param>
        /// <param name="parameters">The parameters of the command.</param>
        /// <returns>The result of executing the command.</returns>
        public async Task<TResult> ExecuteAsync(AuthenticationContext authenticationContext, TParameters parameters)
        {
            if (!authenticationContext.CanExecute(this))
            {
                Logger.LogError("'{CommandId}' not supported with the configured authentication scheme ('{AuthenticationContextId}').", Id, authenticationContext.Id);
                
                throw new CommandNotSupportedException($"'{Id}' not supported with the configured authentication scheme ('{authenticationContext.Id}')");
            }

            var request = BuildRequest(parameters);

            AddAuthorizationHeader(request, authenticationContext);

            var response = await ExecuteRequest(request).ConfigureAwait(false);

            return MapResponse(response);
        }

        private static void AddAuthorizationHeader(HttpRequestMessage request, AuthenticationContext authenticationContext)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                authenticationContext.Token.AccessToken);
        }
    }
}