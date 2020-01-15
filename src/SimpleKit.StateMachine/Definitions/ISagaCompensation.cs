using System;

namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaCompensation
    {
        ISagaStepAndReply AssignCompensation(Func<ISagaCommand,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedStage);
    }
}