using System;
using SagaContract;
using SimpleKit.StateMachine.Definitions;
using SimpleKit.StateMachine.Messaging.RabbitMQ;

namespace OrderWebApi.Domains
{
    public class TripManagementEndpoints
    {
        private RabbitMQBus _rabbitMqBus;

        public SagaCommandEndpoint  CancelHotel(ISagaCommand sagaCommand, BookingTripState sagaState)
        {
            return new NoReplyCommandEndpoint();
        }
        
        public SagaCommandEndpoint BookHotel(ISagaCommand sagaCommand, BookingTripState sagaState)
        {
            var cmd = (BookingTripState.BookHotelCommand) sagaCommand;
            var endpoint = new BookHotelSagaCommandEndpoint(new BookHotelMsg()
            {
                SagaId = cmd.SagaId,
                HotelId = sagaState.HotelBookingId
            });
            endpoint.SagaCommandContext = new SagaCommandContext()
            {
                Host = "OrderService",
                CorrelationId = cmd.SagaId,
                DestinationAddress = "saga-book-hotel-queue",
                ReplyAddress = "saga-book-hotel-queue-reply",
                MessageId = Guid.NewGuid(),
                MessageType = typeof(BookHotelMsg).AssemblyQualifiedName,
                ReplyMessageType = typeof(BookHotelMsgReply).AssemblyQualifiedName,
                FaultMessageType = typeof(BookHotelMsgReplyError).AssemblyQualifiedName
            };
            return endpoint;
        }
        
        public SagaCommandEndpoint CancelCar(ISagaCommand sagaCommand, BookingTripState sagaState)
        {
            return new NoReplyCommandEndpoint();
        }
        public SagaCommandEndpoint BookCar(ISagaCommand sagaCommand, BookingTripState sagaState)
        {
            return new NoReplyCommandEndpoint();
        }
    }
}