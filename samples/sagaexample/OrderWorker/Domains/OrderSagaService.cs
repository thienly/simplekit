using Microsoft.EntityFrameworkCore;
using SimpleKit.StateMachine.Definitions;

namespace OrderWorker.Domains
{
    public class OrderSagaService
    {
        private DbSet<Order> _orders;

        public OrderSagaService(DbSet<Order> orders)
        {
            _orders = orders;
        }

        public SagaCommandEndpoint RejectOrder(ISagaCommand command)
        {
            var orderCommand = (OrderState.RejectCommand) command;
            var order = _orders.Find(orderCommand.OrderId);
            order.Status = "Reject";
            return new NoReplyCommandEndpoint();
        }

        public ISagaReplyCommand ReceiveReply()
        {
            return new KitchenReplyCommand();
        }
    }
}