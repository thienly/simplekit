using System;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Trace.Configuration;
using OpenTelemetry.Trace.Sampler;
using ProductMgt.ApplicationService;
using ProductMgt.DomainEventHandlers;
using ProductMgt.Infrastructure;
using ProductMgt.Shared.MediaApi;
using ProductMgtServices.Middlewares;
using ProductMgtServices.Profiles;
using SimpleKit.Infrastructure.Bus.Kafka;
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
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ProductProfile>();
            },Assembly.GetExecutingAssembly());
            services.Configure<FileSystemOptions>(Configuration.GetSection("FileSystemOptions"));
            services.AddSingleton<IFileSystemUtilities, FileSystemUtilities>();
            services
                .AddDomainEventHandler()
                .AddApplicationService()
                .AddInfrastructure(new EfCoreSqlServerOptions()
                {
                    MainDbConnectionString = "Server=10.0.19.103;Database=ProductMgt;User Id=sa;Password=Test!234"
                })
                .AddKafkaBus(new KafkaOptions()
                {
                    KafkaHost = "10.0.19.103:9092"
                }, new NullLoggerFactory())
                .AddDomainFactory();
                
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Product management"
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
            //app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}