using System;
using SimpleKit.Domain.Entities;

namespace Order.HotelService.Domains
{
    public class RoomReservation : AggregateRootBase
    {
        public Guid RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsBooked { get; set; } = true;
    }
}