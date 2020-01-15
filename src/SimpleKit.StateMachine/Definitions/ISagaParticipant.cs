using System;

namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaParticipant
    {
        ISagaReplyAndCompensation AssignParticipant(Func<ISagaCommand,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedState);
        ISagaStepDefinition AssignCompensation(Func<ISagaCommand,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedState);
    }
}