using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Reddit.NET.Core.Client.Command
{
    public static class CommandExecutorExtensions
    {
        public static async Task<TResponse> ExecuteCommandAsync<TResponse>(
            this CommandExecutor executor, 
            ClientCommand command)
        {
            var response = await executor.ExecuteCommandAsync(command).ConfigureAwait(false);

            return await response
                .Content
                .ReadFromJsonAsync<TResponse>()
                .ConfigureAwait(false);
        }
    }
}