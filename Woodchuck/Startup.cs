using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Woodchuck.Models;

namespace Woodchuck
{
    class Startup
    {
        public IHost CreateHost(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<WoodchuckContext>(o =>
                    o.UseSqlServer(hostContext.Configuration.GetConnectionString("Default")));
                services.AddHostedService<SaverWorker>();
                services.AddHostedService<FetcherWorker>();
                services.AddSingleton<LogQueue>();
                services.AddHttpClient();
            });

            var host = builder.Build();
            Configure(host);

            return host;
        }

        private void Configure(IHost host)
        {
            using var serviceScope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<WoodchuckContext>();
            context.Database.Migrate();
        }
    }
}
