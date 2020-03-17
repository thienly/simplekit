using System;

namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaParticipant
    {
        ISagaReplyAndCompensation AssignParticipant(Func<ISagaCommand,ISagaState,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedState);
        ISagaStepDefinition AssignCompensation(Func<ISagaCommand,ISagaState,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedState);
    }
}