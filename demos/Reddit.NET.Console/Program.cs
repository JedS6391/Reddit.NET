using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reddit.NET.Console.Examples;

namespace Reddit.NET.Console
{
    /// <summary>
    /// The main console example program.
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((_, services) => 
                {
                    services.AddHttpClient();
                    
                    services.AddSingleton<EntryPoint>();

                    services.AddSingleton<IExample, AuthorizationCodeExample>();
                    services.AddSingleton<IExample, UsernamePasswordExample>();
                    services.AddSingleton<IExample, ReadOnlyExample>();
                })
                .Build();

            var entryPoint = host.Services.GetRequiredService<EntryPoint>();

            await entryPoint.RunAsync();
        }
    }
}
