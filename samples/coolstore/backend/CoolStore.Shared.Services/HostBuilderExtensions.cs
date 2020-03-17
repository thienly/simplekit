using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoolStore.Shared.Services
{
    public class HostBuilderExtensions
    {
        public static IHostBuilder CreateCoolStoreWebHostBuilder(Type startupType)
        {
            
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(8081, cfg => { cfg.Protocols = HttpProtocols.Http2; });
                    });
                    builder.UseStartup(startupType); 
                });
        }
    }
    
}