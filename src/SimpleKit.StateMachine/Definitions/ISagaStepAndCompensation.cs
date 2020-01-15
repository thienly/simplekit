using System;

namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaStepAndCompensation :ISagaStepDefinition        
    {
        ISagaStepDefinition AssignCompensation(Func<ISagaCommand,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedStage);
    }
}