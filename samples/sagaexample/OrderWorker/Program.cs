using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using OrderWorker.Domains;
using SimpleKit.StateMachine;
using SimpleKit.StateMachine.Persistences;

namespace OrderWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            
            var serviceCollection = new ServiceCollection();
            var cfgBuilder = new ConfigurationBuilder();
            cfgBuilder.AddJsonFile(Path.Combine(Environment.CurrentDirectory,"appsettings.json"), false);
            var configurationRoot = cfgBuilder.Build();
            var mongoClient = new MongoClient(configurationRoot.GetConnectionString("MongoDb"));
            BsonClassMap.RegisterClassMap<BookingTripState>(map => { map.AutoMap(); });
            serviceCollection.AddSagaManager(new MongoUrl(configurationRoot.GetConnectionString("MongoDb")));
            
            var mongoCollection = mongoClient.GetDatabase("sagamanager").GetCollection<SagaStateProxy>(nameof(SagaStateProxy));
            
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            Console.WriteLine("Input order name");
            var sagaGuids = new List<Guid>();
            using (var scope = serviceProvider.CreateScope())
            {
                //Start the saga
                var bookingTrip = new BookingTrip();
                bookingTrip.BookCarWithId(Guid.Parse("1fac8e32-9414-4494-aae9-b1122ce0a7cd"));
                bookingTrip.BookHotelWithId(Guid.Parse("b36acb57-01d8-46c1-bf25-bbc027446cec"));

                var state = new BookingTripState(bookingTrip, Guid.Parse("d62c95e7-bb55-4c34-95a2-b9ef67158e22"));

                var bookingTripDefinition = new BookingTripDefinition(state);
                SagaStepDefinitionFactory factory = (t) => bookingTripDefinition;
                var sagaManager = new SagaManager(factory, new SagaPersistence(mongoCollection));
                var sagaStateProxy = sagaManager.Process(state);
                mongoCollection.InsertOne(sagaStateProxy);
                Console.WriteLine("Input order name");
            }
        }
    }
}