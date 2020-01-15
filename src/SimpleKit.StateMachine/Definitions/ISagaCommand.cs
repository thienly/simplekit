using System;

namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaCommand
    {
         Guid SagaId { get; set; }
    }
}