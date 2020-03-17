using System;
using SimpleKit.StateMachine.Definitions;

namespace OrderWorker.Domains
{
    public class OrderState : ISagaState
    {
        public Guid SagaId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid TicketId { get; set; }

        public ISagaCommand Reject()
        {
            return new RejectCommand()
            {
                SagaId = this.SagaId,
                OrderId = this.OrderId
            };
        }
        public ISagaCommand AskKitchen()
        {
            return new AskingKitchenCommand()
            {
                OrderId =  this.OrderId,
                Note = "No spicy"
            };
        }

        public ISagaCommand NothingChange()
        {
            return new SuccessCommand();    
        }
        public void Approve(ISagaReplyCommand command)
        {
            var kitchen = (KitchenReplyCommand) command;
            TicketId = kitchen.KitchenId;
        }
        public class RejectCommand : ISagaCommand
        {
            public Guid SagaId { get; set; }
            public Guid OrderId { get; set; }
        }
        public class SuccessCommand: ISagaCommand
        {
            public Guid SagaId { get; set; }
        }
    }
}