using System;
using System.Collections.Generic;
using System.Linq;
using SimpleKit.Domain.Entities;

namespace Order.HotelService.Domains
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }

    public class Room : AggregateRootBase
    {
        public string Name { get; set; }
        public RoomType RoomType { get; set; }
        public decimal Price { get; set; }

        public List<RoomReservation> RoomReservations { get; set; } = new List<RoomReservation>();

        public RoomReservation Booking(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new DomainException("The Start Date can not greeter then End Date");
            if (RoomReservations.Any(d =>
                d.EndDate <= startDate || (d.StartDate <= startDate && d.EndDate >= endDate && d.IsBooked)))
                throw new DomainException($"This care has been booked from {startDate} to {endDate}");
            var reservation = new RoomReservation()
            {
                RoomId = this.Id,
                StartDate = startDate,
                EndDate = endDate
            };
            RoomReservations.Add(reservation);
            return reservation;
        }

        public void Cancel(Guid bookingId)
        {
            var roomReservation = RoomReservations.Single(x => x.Id == bookingId);
            roomReservation.IsBooked = false;
        }

        public decimal GetPricing(DateTime startDate, DateTime endDate)
        {
            return 100;
        }
    }
}