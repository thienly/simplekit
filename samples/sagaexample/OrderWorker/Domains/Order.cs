using System;
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
    public class KitchenReplyCommand : ISagaReplyCommand
    {
        public Guid SagaId { get; set; } 
        public Guid OrderId { get; set; }
        public bool IsSuccess { get; set; }
        public Guid KitchenId { get; set; }
    }
    public class AskingKitchenCommand : ISagaCommand
    {
        public Guid OrderId { get; set; }
        public string Note { get; set; }
        public Guid SagaId { get; set; }
    }
}