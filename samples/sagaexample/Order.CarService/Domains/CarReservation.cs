using System;
using SimpleKit.Domain.Entities;

namespace Order.CarService.Domains
{
    public class CarReservation : AggregateRootBase
    {
        public Guid CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsBooked { get; set; } = true;
    }
}