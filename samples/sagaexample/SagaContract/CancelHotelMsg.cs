using System;

namespace SagaContract
{
    public class CancelHotelMsg
    {
        public Guid SagaId { get; set; }
        public Guid HotelId { get; set; }
    }
}