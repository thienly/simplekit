using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using CarServices;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MongoDB.Driver;
using Order.CarService.Domains;
using Order.CarService.Repositories;
using SimpleKit.Domain.Repositories;
using CarReservation = CarServices.CarReservation;
using Enum = System.Enum;

namespace Order.CarService.Services
{
    public class CarSvc : CarServices.CarSvc.CarSvcBase
    {
        private IMongoQueryRepository<Car> _queryRepository;
        private IRepository<Car> _repository;

        public CarSvc( IRepository<Car> repository, IMongoQueryRepository<Car> queryRepository)
        {
            _repository = repository;
            _queryRepository = queryRepository;
        }

        public override async Task<GetCarReply> GetCar(GetCarRequest request, ServerCallContext context)
        {
            var car = await _queryRepository.GetById(request.Id);
            var reservation = new RepeatedField<CarReservation>();
            reservation.AddRange(car.CarReservations.Select(x=> new CarReservation()
            {
                StartDate = x.StartDate.ToTimestamp(),
                EndDate =  x.EndDate.ToTimestamp(),
                Id = x.Id.ToString()
            }));
            var result = new GetCarReply()
            {
                Id = car.Id.ToString(),
                CarType = car.CarType.ToString(),
                Name = car.Name,
                Price = car.Price,
                CarReservations = {reservation}
            };
            return result;
        }

        public override async Task<AddCarReply> AddCar(AddCarRequest request, ServerCallContext context)
        {
            var car = new Car()
            {
                Name = request.Name,
                CarType = Enum.Parse<CarType>(request.CarType),
                Price = request.Price
            };
            var addedCar = await _repository.AddAsync(car);
            return new AddCarReply()
            {
                Id = addedCar.Id.ToString(),
                Name = addedCar.Name
            };
        }

        public override async Task<BookCarReply> BookCar(BookCarRequest request, ServerCallContext context)
        {
            var endDate = request.EndDate.ToDateTime();
            var startDate = request.StartDate.ToDateTime();
            var car = await _queryRepository.GetById(request.Id);
            try
            {
                var carReservation = car.Booking(startDate,endDate);
                await _repository.UpdateAsync(car);
                return new BookCarReply()
                {
                    CarBookingId = carReservation.Id.ToString(),
                    IsSuccess = true
                };
            }
            catch (DomainException e) 
            {
                return new BookCarReply()
                {
                    IsSuccess = false,
                    Reason = e.Message
                };
            }
            
        }
        public override async Task<CancelCarBookingReply> CancelBooking(CancelCarBookingRequest request, ServerCallContext context)
        {
            try
            {
                var car = await _queryRepository.GetCarByBookingId(request.CarBookingId);
                car.Cancel(request.CarBookingId);
                await _repository.UpdateAsync(car);
                return new CancelCarBookingReply()
                {
                    IsSuccess = true
                };
            }
            catch (DomainException e)
            {
                return new CancelCarBookingReply()
                {
                    IsSuccess = false,
                    Reason = e.Message
                };
            }
        }

        public override async Task<ListOfCarReply> ListOfCar(ListOfCarRequest request, ServerCallContext context)
        {
            var listAsync = await _queryRepository.GetCollection().AsQueryable().ToListAsync();
            var result = new ListOfCarReply();
            listAsync.ForEach(x =>
            {
                result.Cars.Add(new CarMessage()
                {
                    Id =  x.Id.ToString(),
                    Name = x.Name,
                    Price = x.Price,
                    CarType = x.CarType.ToString()
                });
            });
            return result;
        }
    }
}