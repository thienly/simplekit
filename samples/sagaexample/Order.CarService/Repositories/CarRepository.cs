using System.Threading.Tasks;
using MongoDB.Driver;
using Order.CarService.Domains;
using SimpleKit.Domain.Repositories;

namespace Order.CarService.Repositories
{
    public class CarRepository : IRepository<Car>
    {
        private readonly IMongoCollection<Car> _mongoCollection;

        public CarRepository(IMongoCollection<Car> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }
        public async Task<Car> AddAsync(Car entity)
        {
            await _mongoCollection.InsertOneAsync(entity, new InsertOneOptions()
            {
                BypassDocumentValidation = true
            });
            return entity;
        }
        public async Task<Car> UpdateAsync(Car entity)
        {
            var findOneAndReplaceAsync = await _mongoCollection.FindOneAndReplaceAsync<Car,Car>(filter: c => c.Id == entity.Id,
                replacement: entity, new FindOneAndReplaceOptions<Car, Car>()
                {
                    IsUpsert = false
                });
            return findOneAndReplaceAsync;
        }
        public async Task DeleteAsync(Car entity)
        {
            await _mongoCollection.DeleteOneAsync(c => c.Id == entity.Id);
        }
    }
}