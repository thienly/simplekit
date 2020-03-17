using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Order.HotelService.Domains;

namespace Order.HotelService.Repositories
{
    public static class RoomQueryRepositoryExtensions
    {
        public static async Task<Room> GetById(this IMongoQueryRepository<Room> queryRepository, Guid id)
        {
            var filterBuilder = new FilterDefinitionBuilder<Room>();
            var filterDefinition = filterBuilder.Eq(c => c.Id, id);
            var car = (await queryRepository.GetCollection().FindAsync(filterDefinition)).First();
            return car;
        }

        public static async Task<Room> GetRoomByBookingId(this IMongoQueryRepository<Room> queryRepository, Guid bookingId)
        {
            var filterBuilder = new FilterDefinitionBuilder<Room>();
            var filterDefinition = filterBuilder.Where(c => c.RoomReservations.Any(r => r.Id == bookingId));
            var car = (await queryRepository.GetCollection().FindAsync(filterDefinition)).First();
            return car;
        }
    }
}