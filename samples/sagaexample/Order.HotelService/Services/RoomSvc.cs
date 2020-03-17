using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MongoDB.Driver;
using Order.HotelService.Domains;
using Order.HotelService.Repositories;
using RoomServices;
using SimpleKit.Domain.Repositories;
using Enum = System.Enum;
using RoomReservation = RoomServices.RoomReservation;

namespace Order.HotelService.Services
{
    public class RoomSvc : RoomServices.RoomSvc.RoomSvcBase
    {
        private IMongoQueryRepository<Room> _queryRepository;
        private IRepository<Room> _repository;
    
        public RoomSvc( IRepository<Room> repository, IMongoQueryRepository<Room> queryRepository)
        {
            _repository = repository;
            _queryRepository = queryRepository;
        }

        public override async Task<GetRoomReply> GetRoom(GetRoomRequest request, ServerCallContext context)
        {
            var room = await _queryRepository.GetById(request.Id);
            var reservation = new RepeatedField<RoomReservation>();
            reservation.AddRange(room.RoomReservations.Select(x=> new RoomReservation()
            {
                Id = x.Id.ToString(),
                StartDate = x.StartDate.ToTimestamp(),
                EndDate = x.EndDate.ToTimestamp()
            }));
            var result = new GetRoomReply()
            {
                Id = room.Id.ToString(),
                RoomType = room.RoomType.ToString(),
                Name = room.Name,
                Price = room.Price,
                Reservation = { reservation}
            };
            return result;
        }
        
        public override async Task<AddRoomReply> AddRoom(AddRoomRequest request, ServerCallContext context)
        {
            var room = new Room()
            {
                Name = request.Name,
                RoomType = Enum.Parse<RoomType>(request.RoomType),
                Price = request.Price
            };
            var addedRoom = await _repository.AddAsync(room);
            return new AddRoomReply()
            {
                Id = addedRoom.Id.ToString(),
                Name = addedRoom.Name
            };
        }
        
        public override async Task<BookRoomReply> BookRoom(BookRoomRequest request, ServerCallContext context)
        {
            var endDate = request.EndDate.ToDateTime();
            var startDate = request.StartDate.ToDateTime();
            var room = await _queryRepository.GetById(request.Id);
            try
            {
                var roomReservation = room.Booking(startDate,endDate);
                await _repository.UpdateAsync(room);
                return new BookRoomReply()
                {
                    RoomBookingId = roomReservation.Id.ToString(),
                    IsSuccess = true
                };
            }
            catch (DomainException e) 
            {
                return new BookRoomReply()
                {
                    IsSuccess = false,
                    Reason = e.Message
                };
            }
            
        }
        public override async Task<CancelRoomBookingReply> CancelBooking(CancelRoomBookingRequest request, ServerCallContext context)
        {
            try
            {
                var room = await _queryRepository.GetRoomByBookingId(request.RoomBookingId);
                room.Cancel(request.RoomBookingId);
                await _repository.UpdateAsync(room);
                return new CancelRoomBookingReply()
                {
                    IsSuccess = true
                };
            }
            catch (DomainException e)
            {
                return new CancelRoomBookingReply()
                {
                    IsSuccess = false,
                    Reason = e.Message
                };
            }
        }
        
        public override async Task<ListOfRoomReply> ListOfRoom(ListOfRoomRequest request, ServerCallContext context)
        {
            var listAsync = await _queryRepository.GetCollection().AsQueryable().ToListAsync();
            var result = new ListOfRoomReply();
            listAsync.ForEach(x =>
            {
                result.Rooms.Add(new RoomMessage()
                {
                    Id =  x.Id.ToString(),
                    Name = x.Name,
                    Price = x.Price,
                    RoomType = x.RoomType.ToString()
                });
            });
            return result;
        }
    }
}