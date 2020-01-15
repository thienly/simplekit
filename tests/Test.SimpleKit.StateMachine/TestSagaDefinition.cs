using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using SimpleKit.StateMachine;
using SimpleKit.StateMachine.Definitions;
using Xunit;
using Xunit.Abstractions;

namespace Test.SimpleKit.StateMachine
{
    public class TestSagaDefinition
    {
        private ITestOutputHelper _testOutputHelper;

        public TestSagaDefinition(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test_if_saga_definition_configurated_successfully()
        {
            //var orderSagaDef = new OrderSagaDefinitionTest();
            //var x = 0;
            // Begin transaction
            var order = new Order("DonhanTiki");
            var orderState = new OrderSagaState()
            {
                OrderId =  order.OrderId
            };
            SagaStepDefinitionFactory factory = type =>
                new OrderSagaDefinitionTest(orderState, new KitchenService(), new OrderService(order));
            
            saga.StartSaga(orderState);
            // saving saga state same with local transaction.
            // End Transaction
        }
    }

    public enum OrderStatus
    {
        Starting,
        Pending,
        Rejected,
        Approved
        
    }
    public class Order
    {
        public Order(string customerName)
        {
            CustomerName = customerName;
        }

        public Guid OrderId { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Starting;

        public void Reject()
        {
            Status = OrderStatus.Rejected;
        }

        public void Approve()
        {
            Status = OrderStatus.Approved;
        }

        public void Pending()
        {
            Status = OrderStatus.Pending;
        }
    }
    public class OrderSagaState : ISagaState
    {
        public Guid OrderId { get; set; }
        public Guid KitchenId { get; set; }

        public ISagaCommand CreateKitchenOrder()
        {
            return new CreateKitchenOrderCommand(); 
        }
        public ISagaCommand CancelKitchenOrder()
        {
            return new CreateKitchenOrderCommand(); 
        }

        public void UpdateKitchen(ISagaReplyCommand reply)
        {
            
        }

        public ISagaCommand RejectOrder()
        {
            return new RejectOrderCommand();    
        }
        public ISagaCommand PendingOrder()
        {
            return new PendingOrderCommand();
        }

        public Guid Id { get; } = new Guid();
    }

    public class PendingOrderCommand : ISagaCommand
    {
        
    }
    public class RejectOrderCommand : ISagaCommand
    {
        
    }
    public class CreateKitchenOrderCommand : ISagaCommand
    {
        public Guid OrderId { get; set; }            
    }
    public class KitchenReply : ISagaReplyCommand
    {
            
    }
    
    public interface IKitchenService
    {
        void InvokeParticipant(ISagaCommand command); 
        ISagaReplyCommand HandleReply();
    }

    
    public class MemoryQueue 
    {
        private string _name;
        private Queue<dynamic> _queue = new Queue<dynamic>();
        public event Consume Consumer;
        public delegate void Consume(dynamic data);
        public MemoryQueue(string name)
        {
            _name = name;
        }
        
        public void Enqueue(dynamic data)
        {
            _queue.Enqueue(data);
        }

        public dynamic Dequeue()
        {
            return _queue.Dequeue();
        }
        
    }    
    public class KitchenService : IKitchenService
    {
        private MemoryQueue _kitchenQueue = new MemoryQueue("KitchenQueue");
        public void InvokeParticipant(ISagaCommand command)
        {
            // Push to the queue and 
            _kitchenQueue.Enqueue(command);
        }

        public ISagaReplyCommand HandleReply()
        {
            throw new System.NotImplementedException();
        }
    }
    public interface IOrderService
    {
        void Reject(ISagaCommand commnad);
        SagaCommandEndpoint<PendingOrderCommand> Pending(ISagaCommand command);
    }

    
    public class OrderService : IOrderService, ISagaCommandEndpointBuilder
    {
        private Order _order;

        public OrderService(Order order)
        {
            _order = order;
        }

        public void Reject(ISagaCommand commnad)
        {
            // load 
            _order.Reject();
        }

        public SagaCommandEndpoint<PendingOrderCommand> Pending(ISagaCommand commnad)
        {
            return CommandEndpointBuilder<PendingOrderCommand>.BuildCommandFor((PendingOrderCommand) commnad)
                .CommandChannel("CommandChannel")
                .ReplyWithData(typeof(object))
                .Build();
            
        }

        public IReadOnlyCollection<string> GetReplyChannel()
        {
            return new[] {"ReplyCommand"};
        }
    }
    public class OrderSagaDefinitionTest : SagaDefinition<OrderSagaState>
    {
        private IKitchenService _kitchenService;
        private IOrderService _orderService;
        private readonly OrderSagaState _state;

        private ISagaStepDefinition _step;
        public OrderSagaDefinitionTest(OrderSagaState state,IKitchenService kitchenService, IOrderService orderService)
        {
            _state = state;
            _kitchenService = kitchenService;
            _orderService = orderService;
            _step = this.Step("Start")
                .AssignParticipant(cm => _orderService.Pending(cm),_state.PendingOrder)
                .AssignReply(_kitchenService.HandleReply,_state.UpdateKitchen)
                //.AssignCompensation(cm => _orderService.Reject(cm), _state.RejectOrder)
            .Build();
        }

        public override SagaStepDefinition<OrderSagaState> GetStepDefinition()
        {
            return (SagaStepDefinition<OrderSagaState>)_step;
        }
    }
}