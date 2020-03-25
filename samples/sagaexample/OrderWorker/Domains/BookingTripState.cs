using System;
using SimpleKit.StateMachine.Definitions;

namespace OrderWorker.Domains
{
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
}