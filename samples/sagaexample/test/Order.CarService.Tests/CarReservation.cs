using System;
using Order.CarService.Domains;
using ProtoBuf;
using SimpleKit.Domain.Entities;

namespace Order.CarService.Tests
{
    [ProtoContract]
    public class CarReservation : AggregateRootBase
    {
        [ProtoMember(1)]
        public Guid ReservationId { get; set; }
        [ProtoMember(2)]
        public DateTime StartDate { get; set; }
        [ProtoMember(3)]
        public DateTime EndDate { get; set; }
        [ProtoMember(4)]
        public decimal Duration { get; private set; }
    }
}