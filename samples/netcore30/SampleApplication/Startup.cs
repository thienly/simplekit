using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleApplication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddControllersAsServices();
            services.AddTransient<IStartupFilter, SampleStartupFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class SampleMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<SampleMiddleware> _logger;

        public SampleMiddleware(RequestDelegate next, ILogger<SampleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async  Task Invoke(HttpContext context)
        {
            _logger.LogInformation("[TLP] Samplemiddleware logging");
            await _next(context);
        }
    }
    
    public class SampleMiddleware2
    {
        private readonly RequestDelegate _next;
        private ILogger<SampleMiddleware> _logger;

        public SampleMiddleware2(RequestDelegate next, ILogger<SampleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async  Task Invoke(HttpContext context)
        {
            _logger.LogInformation("[TLP] Samplemiddleware2 logging");
            await _next(context);
        }
    }

    public class SampleStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            Action<IApplicationBuilder> result = (builder) =>
            {
                builder.UseMiddleware<SampleMiddleware>();
                next(builder);  
                builder.UseMiddleware<SampleMiddleware2>();
            };
            return result;
        }
    }
}