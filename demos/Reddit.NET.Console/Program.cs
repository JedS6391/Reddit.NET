using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reddit.NET.Client.Builder;
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
                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Debug))
                .ConfigureServices((_, services) => 
                {                    
                    services.AddRedditHttpClient(userAgent: $"{Environment.OSVersion.Platform}:Reddit.NET.Console:v0.1.0 (by JedS6391)");
                
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
