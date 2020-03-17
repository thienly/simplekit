using System.Linq;
using MongoDB.Driver;
using Order.HotelService.Domains;

namespace Order.HotelService.Repositories
{
    public class RoomQueryRepository : IMongoQueryRepository<Room>
    {
        private IMongoCollection<Room> _mongoCollection;

        public RoomQueryRepository(IMongoCollection<Room> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }
        public IQueryable<Room> Queryable()
        {
            return _mongoCollection.AsQueryable();
        }

        public IMongoCollection<Room> GetCollection()
        {
            return _mongoCollection;
        }
    }
}