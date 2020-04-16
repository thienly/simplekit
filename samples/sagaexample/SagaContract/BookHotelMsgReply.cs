using System;

namespace SagaContract
{
    public class BookHotelMsgReply
    {
        public Guid SagaId { get; set; }
        public Guid HotelId { get; set; }
    }
}