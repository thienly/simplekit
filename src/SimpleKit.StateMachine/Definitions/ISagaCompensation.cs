using System;

namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaCompensation
    {
        ISagaStepAndReply AssignCompensation(Func<ISagaCommand,ISagaState,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedStage);
    }
}