using System.Collections.Generic;

namespace SimpleKit.StateMachine.Definitions
{
    public abstract class SagaDefinition
    {
    }

    public abstract class SagaDefinition<TSagaState> : SagaDefinition where TSagaState : class
    {
        protected ISagaParticipant Step(string name)
        {
            var firstStep = new SagaStepDefinition<TSagaState>();
            return firstStep.Step(name);
        }

        public abstract SagaStepDefinition<TSagaState> GetStepDefinition();

        public SagaStepDefinition<TSagaState> GetNextStep(string currentStep)
        {
            var definition = GetStepDefinition();
            while (definition != null)
            {
                if (definition.Name == currentStep)
                    return definition.NextStep;
                definition = definition.NextStep;
            }

            return null;
        }

        public SagaStepDefinition<TSagaState> GetPreviousStep(string currentStep)
        {
            var definition = GetStepDefinition();
            while (definition != null)
            {
                if (definition.Name == currentStep)
                    return definition.PreviousStep;
                definition = definition.NextStep;
            }

            return null;
        }

        public string GetNextStepName(string currentStep)
        {
            var definition = GetStepDefinition();
            while (definition != null)
            {
                if (definition.Name == currentStep)
                {
                    if (definition.NextStep != null)
                        return definition.NextStep.Name;
                    else
                        return definition.Name;
                }

                definition = definition.NextStep;
            }

            return string.Empty;
        }

        public string GetPreviousStepName(string currentStep)
        {
            var definition = GetStepDefinition();
            while (definition != null)
            {
                if (definition.Name == currentStep)
                {
                    if (definition.PreviousStep != null)
                        return definition.PreviousStep.Name;
                    else
                        return definition.Name;
                }

                definition = definition.NextStep;
            }

            return string.Empty;
        }

        public SagaStepDefinition<TSagaState> GetStepWithName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return GetFirstStep();
            var definition = GetStepDefinition();
            while (definition != null)
            {
                if (definition.Name == name)
                    return definition;
                definition = definition.NextStep;
            }

            return null;
        }

        public bool IsCompleted(string currentStep, SagaDirection direction)
        {
            if (direction == SagaDirection.Forward)
                return GetNextStep(currentStep) == null;
            if (direction == SagaDirection.Backward)
                return GetPreviousStep(currentStep) == null;
            return true;
        }

        public SagaStepDefinition<TSagaState> GetFirstStep()
        {
            return GetStepDefinition();
        }

        public SagaStepDefinition<TSagaState> GetLastStep()
        {
            var firstStep = GetStepDefinition();
            while (firstStep.NextStep != null)
            {
                firstStep = firstStep.NextStep;
            }

            return firstStep;
        }
    }

    public interface ISagaCommandEndpointBuilder
    {
        IReadOnlyCollection<string> GetReplyChannel();
    }
}