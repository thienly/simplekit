using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Order.Shared;
using OrderWorker.Domains;
using OrderWorker.EntityConfigurations;
using SimpleKit.Infrastructure.Repository.EfCore.SqlServer;
using SimpleKit.StateMachine;
using SimpleKit.StateMachine.Consumer;
using SimpleKit.StateMachine.Definitions;

namespace OrderWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddScoped<IDbContextTransaction>((x) =>
                x.GetService<OrderDbContext>().Database.BeginTransaction());
            serviceCollection.AddLogging();
            serviceCollection.AddDomainEventHandler().AddInfrastructure(new EfCoreSqlServerOptions()
            {
                MainDbConnectionString = "Server=10.0.19.103;Database=OrderMgt;User Id=sa;Password=Test!234"
            });
            serviceCollection.AddScoped<ISagaPublisher, SagaPublisher>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            Console.WriteLine("Input order name");
            
            var sagaGuids = new List<Guid>();
            while (true)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    //Start the saga
                    var orderName = Console.ReadLine();
                    
                    var orderDbContext = scope.ServiceProvider.GetService<OrderDbContext>();
                    var order = new Domains.Order(orderName);
                    var orderState = new OrderState()
                    {
                        OrderId = order.Id
                    };
                    var definition =
                        new OrderSagaDefinition(new OrderSagaService(orderDbContext.Set<Domains.Order>()), new KitchenSagaService(), orderState);
                    SagaStepDefinitionFactory factory = type =>
                        definition;
                    
                    var sagaPersistence = new SagaPersistence(orderDbContext.Set<SagaStateProxy>());
                    var saga = new SagaManager(factory, sagaPersistence, new SagaPublisher());
                    if (orderName == "Reply")
                    {
                        // starting the reply flow
                        var rd = new Random();
                        var next = rd.Next(0, sagaGuids.Count - 1);
                        var kitchenReplyMessage = new KitchenReplyCommand()
                        {
                            SagaId = sagaGuids[next],
                            IsSuccess = false,
                            OrderId = Guid.NewGuid(),
                            KitchenId =  Guid.NewGuid()
                        };
                        saga.ProcessOnReply(kitchenReplyMessage);
                        orderDbContext.SaveChanges();
                        Console.WriteLine("Input order name");
                        continue;
                    }

                    if (orderName == "Next")
                    {
                        //var rd = new Random();
                        //var next = rd.Next(0, sagaGuids.Count - 1);
                        saga.Process(new OrderState()
                        {
                            SagaId = Guid.Parse("7e3bfc7d-9a91-4f2a-8863-5b456fc12d3a"),
                            OrderId = Guid.Parse("ae754fea-4452-4dd7-903b-c92875cc4f88")
                        });
                        orderDbContext.SaveChanges();
                        Console.WriteLine("Input order name");
                        continue;
                    }
                    
                    saga.Process(orderState);
                    sagaGuids.Add(orderState.SagaId);
                    orderDbContext.Set<Domains.Order>().Add(order);
                    orderDbContext.SaveChanges();
                    //
                    Console.WriteLine("Input order name");
                }
            }
        }
    }
}