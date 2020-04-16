using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using SimpleKit.StateMachine;
using SimpleKit.StateMachine.Persistences;

namespace OrderWebApi.Domains
{
    public interface IDocumentMessage
    {
        Guid Id { get; set; }
    }

    public class DocumentMessageEqualityComparer : IEqualityComparer<IDocumentMessage>
    {
        public bool Equals(IDocumentMessage x, IDocumentMessage y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IDocumentMessage obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public abstract class DocumentBase
    {
        public Guid Id { get; set; }

        private HashSet<IDocumentMessage> _outbox
            = new HashSet<IDocumentMessage>(new DocumentMessageEqualityComparer());

        private HashSet<IDocumentMessage> _inbox
            = new HashSet<IDocumentMessage>(new DocumentMessageEqualityComparer());

        public IEnumerable<IDocumentMessage> Outbox
        {
            get => _outbox;
            protected set => _outbox = value == null
                ? new HashSet<IDocumentMessage>(new DocumentMessageEqualityComparer())
                : new HashSet<IDocumentMessage>(value, new DocumentMessageEqualityComparer());
        }

        public IEnumerable<IDocumentMessage> Inbox
        {
            get => _inbox;
            protected set => _inbox = value == null
                ? new HashSet<IDocumentMessage>(new DocumentMessageEqualityComparer())
                : new HashSet<IDocumentMessage>(value, new DocumentMessageEqualityComparer());
        }
    }

    public class Order
    {
        public Guid Id { get; set; }
        private readonly string _name;

        public Order(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public Guid CarId { get; set; }
        public Guid HotelId { get; set; }

        public void BookHotel(Guid hotelId)
        {
            HotelId = hotelId;
        }

        public void BookCar(Guid carId)
        {
            CarId = carId;
        }

        public string Status { get; set; }

        public void Finalize(Guid carId, Guid hotelId)
        {
            CarId = carId;
            HotelId = hotelId;
            Status = "Pending";
        }

        public List<SagaStateProxy> States { get; set; } = new List<SagaStateProxy>();

        public void AddState(SagaStateProxy sagaStateProxy)
        {
            States.Add(sagaStateProxy);
        }
    }

    public interface IDatabaseAdapter<T>
    {
        void Save(T item);
        void Update(T item);
        IEnumerable<Order> GetUncompletedOrders();
    }

    public class MongoDbAdapter : IDatabaseAdapter<Order>
    {
        private string _connectionString;
        private IMongoCollection<Order> _mongoCollection;

        public MongoDbAdapter(string connectionString)
        {
            _connectionString = connectionString;
            var url = new MongoUrl(_connectionString);
            var mongoClient = new MongoClient(url);
            _mongoCollection = mongoClient.GetDatabase(url.DatabaseName).GetCollection<Order>("orders");
        }

        public void Save(Order item)
        {
            _mongoCollection.InsertOne(item);
        }

        public void Update(Order item)
        {
            var builder = new UpdateDefinitionBuilder<Order>();
            var updateDefinition = builder.Set(x => x.States, item.States);
            _mongoCollection.UpdateOne(o => o.Id == item.Id, updateDefinition);
        }

        public IEnumerable<Order> GetUncompletedOrders()
        {
            var filterDefinitionBuilder = Builders<Order>.Filter;

            var completedDefinition = filterDefinitionBuilder.All("States", new BsonArray()
            {
                new BsonDocument()
                {
                    {"$elemMatch", new BsonDocument("IsCompleted", false)}
                }
            });

            var waitingDefinition = filterDefinitionBuilder.All("States", new BsonArray()
            {
                new BsonDocument()
                {
                    {
                        "$elemMatch", new BsonDocument("NextState", new BsonDocument()

                            {{"$regex", "WaitingFor"}, {"$options", "m"}}
                        )
                    }
                }
            });
            var notWaitingDefinition = filterDefinitionBuilder.Not(waitingDefinition);
            return _mongoCollection.Find(completedDefinition & notWaitingDefinition).ToList();
        }
    }

    public class OrderServices
    {
        private IDatabaseAdapter<Order> _databaseAdapter;
        private ISagaManager<BookingTripState> _sagaManager;

        public OrderServices(IDatabaseAdapter<Order> databaseAdapter, ISagaManager<BookingTripState> sagaManager)
        {
            _databaseAdapter = databaseAdapter;
            _sagaManager = sagaManager;
        }

        public Order CreateOrder(string name, Guid carId, Guid hotelId)
        {
            var order = new Order(name);
            order.Finalize(carId, hotelId);
            var orderProxy =
                _sagaManager.Process(new BookingTripState(order.CarId, order.HotelId), new EmptySagaState());
            order.AddState(orderProxy);
            _databaseAdapter.Save(order);
            return order;
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderConfiguration(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.AddScoped(typeof(IDatabaseAdapter<Order>),
                provider => new MongoDbAdapter(configuration.GetConnectionString("MongoDb")));
            serviceCollection.AddScoped<OrderServices>();
            serviceCollection.AddScoped<ISagaManager<BookingTripState>, SagaManager<BookingTripState>>();
            serviceCollection.AddScoped<ISagaPersistence>(x =>
                new SagaPersistence(configuration.GetConnectionString("MongoDb")));
            return serviceCollection;
        }
    }
}