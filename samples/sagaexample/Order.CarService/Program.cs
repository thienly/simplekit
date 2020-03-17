using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace Order.CarService
{
    public class Program
    {
        static void Main(string[] args)
        {
            var defaultHostBuilder = CreateDefaultHostBuilder(args);
            defaultHostBuilder.Run();
        }

        private static IHost CreateDefaultHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(cfg =>
                {
                    cfg.UseStartup<Startup>();
                    cfg.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(8081, cfg => cfg.Protocols = HttpProtocols.Http2);
                    });
                });
            return hostBuilder.Build();
        }
    }
}