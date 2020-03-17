using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Order.CarService.Domains;
using SimpleKit.Domain.Repositories;

namespace Order.CarService.Repositories
{
    public static class CarQueryRepositoryExtensions
    {
        public static async Task<Car> GetById(this IMongoQueryRepository<Car> queryRepository, Guid id)
        {
            var filterBuilder = new FilterDefinitionBuilder<Car>();
            var filterDefinition = filterBuilder.Eq(c => c.Id, id);
            var car = (await queryRepository.GetCollection().FindAsync(filterDefinition)).First();
            return car;
        }

        public static async Task<Car> GetCarByBookingId(this IMongoQueryRepository<Car> queryRepository, Guid bookingId)
        {
            var filterBuilder = new FilterDefinitionBuilder<Car>();
            var filterDefinition = filterBuilder.Where(c => c.CarReservations.Any(r => r.Id == bookingId));
            var car = (await queryRepository.GetCollection().FindAsync(filterDefinition)).First();
            return car;
        }
    }
}