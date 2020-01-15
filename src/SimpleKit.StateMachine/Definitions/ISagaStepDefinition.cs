namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaStepDefinition
    {
        ISagaParticipant Step(string name);
        ISagaStepDefinition Build();
        
    }
}