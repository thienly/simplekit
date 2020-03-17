using System.Linq;
using MongoDB.Driver;
using Order.CarService.Domains;
using SimpleKit.Domain.Repositories;

namespace Order.CarService.Repositories
{
    public class CarQueryRepository : IMongoQueryRepository<Car>
    {
        private IMongoCollection<Car> _mongoCollection;

        public CarQueryRepository(IMongoCollection<Car> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }
        public IQueryable<Car> Queryable()
        {
            return _mongoCollection.AsQueryable();
        }

        public IMongoCollection<Car> GetCollection()
        {
            return _mongoCollection;
        }
    }
}