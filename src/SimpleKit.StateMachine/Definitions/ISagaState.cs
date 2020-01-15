using System;

namespace SimpleKit.StateMachine.Definitions
{
    
    public interface ISagaState
    {
        Guid SagaId { get; set; }
    }
}