using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;
using ProtoBuf;
using SimpleKit.Domain.Entities;

namespace Order.CarService.Tests
{
    public class DomainException : Exception
    {
        public DomainException(string message): base(message)
        {
        }
        
    }
    [ProtoContract]
    public class Car : AggregateRootBase
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public CharType CarType { get; set; }
        [ProtoMember(3)]
        public decimal  Price { get; set; }
        [ProtoMember(4)]
        public IReadOnlyCollection<CarReservation> CarReservations = new List<CarReservation>();
        
        public void Booking(DateTime startDate, DateTime endDate)
        {
            throw new DomainException($"Can not book this care from {startDate} to {endDate}");
        }
        public void Cancel(Guid bookingId)
        {
            
        }

        public decimal GetPricing(DateTime startDate, DateTime endDate)
        {
            return default(decimal);
        }
    }
}