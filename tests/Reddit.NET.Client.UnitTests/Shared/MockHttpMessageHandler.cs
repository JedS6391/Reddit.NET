using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Reddit.NET.Client.UnitTests.Shared
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        public MockHttpMessageHandler()
        {
        }

        public int RequestCount { get; private set; } = 0;

        public Func<HttpRequestMessage, Task<HttpResponseMessage>> RequestFunc { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;

            var response = await RequestFunc.Invoke(request);

            response.RequestMessage = request;

            return response;
        }
    }
}
