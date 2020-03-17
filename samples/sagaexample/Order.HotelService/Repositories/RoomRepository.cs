using System.Threading.Tasks;
using MongoDB.Driver;
using Order.HotelService.Domains;
using SimpleKit.Domain.Repositories;

namespace Order.HotelService.Repositories
{
    public class RoomRepository : IRepository<Room>
    {
        private readonly IMongoCollection<Room> _mongoCollection;

        public RoomRepository(IMongoCollection<Room> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }
        public async Task<Room> AddAsync(Room entity)
        {
            await _mongoCollection.InsertOneAsync(entity, new InsertOneOptions()
            {
                BypassDocumentValidation = true
            });
            return entity;
        }
        public async Task<Room> UpdateAsync(Room entity)
        {
            var findOneAndReplaceAsync = await _mongoCollection.FindOneAndReplaceAsync<Room,Room>(filter: c => c.Id == entity.Id,
                replacement: entity, new FindOneAndReplaceOptions<Room, Room>()
                {
                    IsUpsert = false
                });
            return findOneAndReplaceAsync;
        }
        public async Task DeleteAsync(Room entity)
        {
            await _mongoCollection.DeleteOneAsync(c => c.Id == entity.Id);
        }
    }
}