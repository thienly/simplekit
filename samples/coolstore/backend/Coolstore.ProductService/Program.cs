using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoolStore.Shared.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Coolstore.ProductService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var coolStoreWebHostBuilder = HostBuilderExtensions.CreateCoolStoreWebHostBuilder(typeof(Startup));
            var host = coolStoreWebHostBuilder.Build();
            await host.RunAsync();
        }

    }
}