using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Reddit.NET.Core.Client.Command.Models.Internal;

namespace Reddit.NET.Core.Client.Command.Abstract
{
    internal static class ResponseMappers
    {
        public static Func<HttpContent, Task<TResponse>> DefaultResponseMapper<TResponse>() =>
            (content) => content.ReadFromJsonAsync<TResponse>();

        public static async Task<Comment.Listing> SubmissionCommentsMapper(HttpContent content)
        {
            var rawJsonStream = await content.ReadAsStreamAsync();
            using var json = await JsonDocument.ParseAsync(rawJsonStream);

            if (json.RootElement.GetArrayLength() != 2)
            {
                // TODO: Exception type
                throw new Exception();
            }

            var commentListingElement = json.RootElement[1];
            var commentListingJson = commentListingElement.ToString();

            return JsonSerializer.Deserialize<Comment.Listing>(commentListingJson);
        }
    }
}