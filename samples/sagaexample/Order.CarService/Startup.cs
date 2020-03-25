using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CarServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace.Configuration;
using OpenTelemetry.Trace.Export;
using OpenTelemetry.Trace.Samplers;
using Order.CarService.Domains;
using Order.CarService.Repositories;
using SimpleKit.Domain.Repositories;

namespace Order.CarService
{

    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenTelemetry(builder =>
            {
                builder
                    .SetSampler(new AlwaysSampleSampler())
                    .UseZipkin(options => { options.Endpoint = new Uri("http://45.118.148.55:9411/api/v2/spans"); })

                    // you may also configure request and dependencies collectors
                    .AddRequestCollector()
                    .AddDependencyCollector()
                    
                    .SetResource(Resources.CreateServiceResource("car-services"));
            });
            CarClassMap.Customize();
            services.AddGrpc();
            services.AddGrpcReflection();
            services.AddScoped<IMongoQueryRepository<Car>, CarQueryRepository>();
            services.AddScoped<IRepository<Car>, CarRepository>();
            services.AddScoped<IMongoCollection<Car>>(f =>
            {
                var mongoUrl = new MongoUrl(_configuration["ConnectionString:MongoDb"]);
                var mongoDb = new MongoClient(mongoUrl);
                var mongoDatabase = mongoDb.GetDatabase("carservice");
                return mongoDatabase.GetCollection<Car>(nameof(Car));
            });
        }
    
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseEndpoints(cfg =>
            {
                cfg.MapGrpcReflectionService();
                cfg.MapGrpcService<Order.CarService.Services.CarSvc>();
            });
        }
    }
}