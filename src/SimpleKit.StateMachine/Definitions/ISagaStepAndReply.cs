using System;

namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaStepAndReply : ISagaStepDefinition
    {
        ISagaStepDefinition AssignReply(Func<ISagaReplyCommand> replyCommand, Action<ISagaReplyCommand> sagaStage);
    }
}