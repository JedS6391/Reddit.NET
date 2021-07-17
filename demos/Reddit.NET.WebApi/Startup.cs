using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reddit.NET.Client.Authentication.Abstract;
using Reddit.NET.Client.Authentication.Storage;
using Reddit.NET.Client.Builder;
using Reddit.NET.Client.Command;
using Reddit.NET.WebApi.Services;
using Reddit.NET.WebApi.Services.Interfaces;

namespace Reddit.NET.WebApi
{
    /// <summary>
    /// Configures the Web API.
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddRedditHttpClient(userAgent: $"{Environment.OSVersion.Platform}:Reddit.NET.WebApi:v0.1.0 (by JedS6391)");

            services.AddHttpContextAccessor();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.Configure<RedditOptions>(options =>
            {
                options.ClientId = Configuration.GetValue<string>("REDDIT_CLIENT_ID");
                options.ClientSecret = Configuration.GetValue<string>("REDDIT_CLIENT_SECRET");
                options.RedirectUri = Configuration.GetValue<Uri>("REDDIT_CLIENT_REDIRECT_URI");
            });

            services.AddSingleton<CommandExecutor>();
            services.AddSingleton<ITokenStorage, MemoryTokenStorage>();
            services.AddSingleton<ISessionService, SessionService>();
            services.AddSingleton<IRedditService, RedditService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
