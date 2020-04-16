using System;

namespace SagaContract
{
    public class BookHotelMsgReplyError
    {
        public Guid SagaId { get; set; }
        public string ErrorMessage { get; set; }
    }
}