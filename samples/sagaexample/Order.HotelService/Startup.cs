using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Order.HotelService.Consumers;
using Order.HotelService.Domains;
using Order.HotelService.Repositories;
using Order.HotelService.Services;
using SimpleKit.Domain.Repositories;

namespace Order.HotelService
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
            RoomClassMap.Customize();
            services.AddGrpc();
            services.AddGrpcReflection();
            services.AddScoped<IMongoQueryRepository<Room>, RoomQueryRepository>();
            services.AddScoped<IRepository<Room>, RoomRepository>();
            services.AddSagaConsumer(_configuration);
            services.AddHostedService<SagaBackgroundService>();
            services.AddScoped<IMongoCollection<Room>>(f =>
            {
                var mongoUrl = new MongoUrl(_configuration["ConnectionString:MongoDb"]);
                
                var mongoDb = new MongoClient(mongoUrl);
                var mongoDatabase = mongoDb.GetDatabase("hotelservice");
                return mongoDatabase.GetCollection<Room>(nameof(Room));
            });
        }
    
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IHostApplicationLifetime hostApplicationLifetime)
        {
            
            app.UseRouting();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseEndpoints(cfg =>
            {
                cfg.MapGrpcReflectionService();
                cfg.MapGrpcService<RoomSvc>();
            });
            
        }

        
    }
}