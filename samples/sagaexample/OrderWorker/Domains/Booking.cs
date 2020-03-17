using System;
using CarServices;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using RoomServices;
using SimpleKit.StateMachine.Definitions;

namespace OrderWorker.Domains
{
    public class BookingTrip
    {
        public Guid CarId { get; private set; }
        public Guid HotelId { get; private set; }
        public string Status { get; private set; }

        public void BookCarWithId(Guid id)
        {
            CarId = id;
        }

        public void BookHotelWithId(Guid id)
        {
            HotelId = id;
        }

        public void SetStatus(string status)
        {
            Status += status;
        }
    }

    public class BookingTripState : ISagaState
    {
        public Guid SagaId { get; set; }
        public Guid CarBookingId { get; set; }
        public Guid HotelBookingId { get; set; }
        public BookingTrip Trip { get; set; }

        public BookingTripState(BookingTrip trip) : this(trip, Guid.NewGuid())
        {
        }

        public BookingTripState(BookingTrip trip, Guid id)
        {
            Trip = trip;
            SagaId = id;
        }

        public BookingTripState()
        {
        }

        public class CancelHotelCommand : ISagaCommand
        {
            public Guid SagaId { get; set; }
            public Guid HotelId { get; set; }
        }

        public class BookHotelCommand : ISagaCommand
        {
            public Guid SagaId { get; set; }
            public Guid HotelId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public CancelHotelCommand CancelHotel()
        {
            return new CancelHotelCommand()
            {
                HotelId = this.HotelBookingId,
                SagaId = this.SagaId
            };
        }

        public BookHotelCommand BookHotel()
        {
            return new BookHotelCommand()
            {
                SagaId = this.SagaId,
                HotelId = this.Trip.HotelId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };
        }

        public class CancelCarCommand : ISagaCommand
        {
            public Guid SagaId { get; set; }
            public Guid CarId { get; set; }
        }

        public class BookCarCommand : ISagaCommand
        {
            public Guid SagaId { get; set; }
            public Guid CarId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public CancelCarCommand CancelCar()
        {
            return new CancelCarCommand()
            {
                CarId = this.CarBookingId,
                SagaId = this.SagaId
            };
        }

        public BookCarCommand BookCar()
        {
            return new BookCarCommand()
            {
                SagaId = this.SagaId,
                CarId = this.Trip.CarId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };
        }
    }

    public class BookingCommandHandler
    {
        private CarSvc.CarSvcClient _carSvcClient;
        private RoomSvc.RoomSvcClient _roomSvcClient;

        public BookingCommandHandler()
        {
            var carChannel = GrpcChannel.ForAddress("http://localhost:8081");
            _carSvcClient = new CarSvc.CarSvcClient(carChannel);
            var hotelChannel = GrpcChannel.ForAddress("http://localhost:8082");
            _roomSvcClient = new RoomSvc.RoomSvcClient(hotelChannel);
        }

        public SagaCommandEndpoint CancelHotel(ISagaCommand sagaCommand, BookingTripState bookingTripState)
        {
            var cmd = (BookingTripState.CancelHotelCommand) sagaCommand;

            // to cancel the booking
            // update state of saga.
            // update booking order.
            return new NoReplyCommandEndpoint();
        }

        public SagaCommandEndpoint BookHotel(ISagaCommand sagaCommand, BookingTripState bookingTripState)
        {
            var cmd = (BookingTripState.BookHotelCommand) sagaCommand;
            try
            {
                var bookRoomReply = _roomSvcClient.BookRoom(new BookRoomRequest()
                {
                    Id = new RoomServices.UUID()
                    {
                        Value = cmd.HotelId.ToString()
                    },
                    StartDate = cmd.StartDate.ToTimestamp(),
                    EndDate = cmd.EndDate.ToTimestamp()
                });
                bookingTripState.HotelBookingId = Guid.Parse(bookRoomReply.RoomBookingId.Value);
                bookingTripState.Trip.SetStatus("Booked Hotel");
                return new NoReplyCommandEndpoint();
            }
            catch (RpcException e)
            {
                return new NoReplyCommandEndpoint();
            }
        }

        public SagaCommandEndpoint CancelCar(ISagaCommand sagaCommand, BookingTripState bookingTripState)
        {
            var cmd = (BookingTripState.CancelCarCommand) sagaCommand;
            // to cancel the booking
            // update state of saga.
            // update booking order.
            return new NoReplyCommandEndpoint();
        }

        public SagaCommandEndpoint BookCar(ISagaCommand sagaCommand, BookingTripState bookingTripState)
        {
            var cmd = (BookingTripState.BookCarCommand) sagaCommand;
            // book a car update bookingtripstate and order trip.

            var bookCarReply = _carSvcClient.BookCar(new BookCarRequest()
            {
                Id = new CarServices.UUID()
                {
                    Value = cmd.CarId.ToString()
                },
                StartDate = cmd.StartDate.ToTimestamp(),
                EndDate = cmd.EndDate.ToTimestamp()
            });
            bookingTripState.CarBookingId = Guid.Parse(bookCarReply.CarBookingId.Value);
            bookingTripState.Trip.SetStatus("Booked Car");
            return new NoReplyCommandEndpoint();
        }
    }

    public class BookingTripDefinition : SagaDefinition<BookingTripState>
    {
        private ISagaStepDefinition _definition;
        private BookingCommandHandler _bookingCommandHandler;
        private BookingTripState _bookingTripState;

        public BookingTripDefinition(BookingTripState bookingTripState)
        {
            _bookingCommandHandler = new BookingCommandHandler();
            _bookingTripState = bookingTripState;
            _definition = this.Step("BeginBooking")
                .AssignCompensation((cm,state) => _bookingCommandHandler.CancelHotel(cm, (BookingTripState)state),
                    _bookingTripState.CancelHotel)
                .Step("BookingHotel")
                .AssignParticipant((command,state) => _bookingCommandHandler.BookHotel(command, (BookingTripState)state),
                    _bookingTripState.BookHotel)
                .AssignCompensation((command,state) => _bookingCommandHandler.CancelCar(command, (BookingTripState)state),
                    _bookingTripState.CancelCar)
                .Step("BookCar")
                .AssignParticipant((command,state) => _bookingCommandHandler.BookCar(command, (BookingTripState)state),
                    _bookingTripState.BookCar)
                .Build();
        }

        public override SagaStepDefinition<BookingTripState> GetStepDefinition()
        {
            return (SagaStepDefinition<BookingTripState>) _definition;
        }
    }
}