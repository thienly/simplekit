using System;
using CarServices;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using RoomServices;
using SimpleKit.StateMachine.Definitions;

namespace OrderWorker.Domains
{
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
}