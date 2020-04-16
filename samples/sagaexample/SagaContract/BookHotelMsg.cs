using System;

namespace SagaContract
{
    public class BookHotelMsg
    {
        public Guid SagaId { get; set; }
        public Guid HotelId { get; set; }
    }
}