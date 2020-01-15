using System;

namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaReplyCommand
    {
        Guid SagaId { get; set; }
        bool IsSuccess { get; set; }
    }
    
    public interface ISagaReply
    {
        ISagaStepAndCompensation AssignReply(Func<ISagaReplyCommand> replyCommand, Action<ISagaReplyCommand> sagaStateHandle);
    }
}