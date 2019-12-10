using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace.Configuration;
using OpenTelemetry.Trace.Sampler;
using ProductMgt.ApplicationService;
using ProductMgt.Infrastructure;
using ProductMgtServices.Middlewares;
using SimpleKit.Infrastructure.Repository.EfCore.SqlServer;

namespace ProductMgtServices
{
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
            
            services.AddApplicationService()
                .AddInfrastructure(new EfCoreSqlServerOptions()
                {
                    MainDbConnectionString = "Server=10.0.19.103;Database=ProductMgt;User Id=sa;Password=Test!234"
                }).AddDomainFactory();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "Product Service Management",
                    Version = "v1"
                });
            });
            services.AddOpenTelemetry(builder =>
            {
                builder
                    .SetSampler(Samplers.AlwaysSample)
                    .UseZipkin(o =>
                    {
                        o.ServiceName = "ProductMgtService";
                        o.Endpoint = new Uri("http://10.0.19.103:9411/api/v2/spans");
                    })
                    .AddRequestCollector()
                    .AddDependencyCollector();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseMiddleware<LogHandlerMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Service Management");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); 
            });
        }
    }
}