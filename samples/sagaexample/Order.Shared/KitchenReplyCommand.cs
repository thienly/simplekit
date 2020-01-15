using System;
using SimpleKit.StateMachine;
using SimpleKit.StateMachine.Definitions;

namespace Order.Shared
{
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