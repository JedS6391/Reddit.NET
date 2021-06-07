using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Reddit.NET.Example
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var host = Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((_, services) => 
                {
                    services.AddHttpClient();
                    services.AddHostedService<Example>();
                });

            return host.RunConsoleAsync();
        }
    }
}
