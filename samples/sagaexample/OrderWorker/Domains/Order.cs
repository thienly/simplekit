using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using Order.Shared;
using SimpleKit.StateMachine.Definitions;

namespace OrderWorker.Domains
{
    public class Order 
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        private readonly string _name;

        public Order(string name)
        {
            _name = name;
        }

        public string Name => _name;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Status { get; set; }
    }

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
    
    public class KitchenSagaService: ISagaCommandEndpointBuilder
    {
        public SagaCommandEndpoint AskKitchen(ISagaCommand command)
        {
            return CommandEndpointBuilder<AskingKitchenCommand>
                .BuildCommandFor((AskingKitchenCommand) command)
                .CommandChannel("Order_Kitchen")
                .ReplyWithData(typeof(KitchenReplyCommand))
                .Build();
        }

        public IReadOnlyCollection<string> GetReplyChannel()
        {
            return new[] {"KitchenReply"};
        }
    }
    
    public class OrderSagaDefinition : SagaDefinition<OrderState>
    {
        private OrderSagaService _orderSagaService;
        private KitchenSagaService _kitchenSagaService;
        private OrderState _orderState;
        private ISagaStepDefinition _definition;
        public OrderSagaDefinition(OrderSagaService orderSagaService, KitchenSagaService kitchenSagaService, OrderState orderState)
        {
            _orderSagaService = orderSagaService;
            _kitchenSagaService = kitchenSagaService;
            _orderState = orderState;
            // Definitions
            _definition = this
                .Step("Getting Approval")
                .AssignCompensation(cm => orderSagaService.RejectOrder(cm),_orderState.Reject)
                .Step("Dont waiting")
                .AssignParticipant(cm => new NoReplyCommandEndpoint(), _orderState.NothingChange)
                .Step("Asking Kitchen Service")
                .AssignParticipant(cm => _kitchenSagaService.AskKitchen(cm), _orderState.AskKitchen)
                .AssignReply(_orderSagaService.ReceiveReply, _orderState.Approve).Build();
        }

        public override SagaStepDefinition<OrderState> GetStepDefinition()
        {
            return (SagaStepDefinition<OrderState>)_definition;
        }
        
    }
}