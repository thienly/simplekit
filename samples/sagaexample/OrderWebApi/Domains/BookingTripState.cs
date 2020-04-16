using System;
using SimpleKit.StateMachine.Definitions;

namespace OrderWebApi.Domains
{
    public class BookingTripState : ISagaState
    {
        public Guid SagaId { get; set; }
        public Guid CarBookingId { get; set; }
        public Guid HotelBookingId { get; set; }
        public BookingTripState(Guid carBookingId, Guid hotelBookingId)
        {
            CarBookingId = carBookingId;
            HotelBookingId = hotelBookingId;
            SagaId = Guid.NewGuid();
        }
        internal BookingTripState()
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
                HotelId = this.HotelBookingId,
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
                CarId = this.CarBookingId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };
        }
    }
}