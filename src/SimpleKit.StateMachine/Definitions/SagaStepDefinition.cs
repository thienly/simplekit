using System;

namespace SimpleKit.StateMachine.Definitions
{
    public class EmptySagaCommand : ISagaCommand
    {
        public Guid SagaId { get; set; }
    }
    public abstract class SagaStepDefinition
    {
        public string Name { get; protected  set; }

        public Func<ISagaCommand,SagaCommandEndpoint> ParticipantHandler { get; protected set; } = command => new NoReplyCommandEndpoint();
        public Func<ISagaCommand>  ParticipantStage { get; protected  set; } = () => new EmptySagaCommand();
        public Func<ISagaCommand,SagaCommandEndpoint> CompensationHandler { get; protected set; } = command => new NoReplyCommandEndpoint();
        public Func<ISagaCommand> CompensationStage { get; protected  set; } = () => new EmptySagaCommand();
        public Func<ISagaReplyCommand> ReplyHandler { get; protected  set; }
        public Action<ISagaReplyCommand> ReplyStage { get; protected  set; }
        
    }
        
    public class SagaStepDefinition<TSagaState> : SagaStepDefinition, ISagaParticipant, ISagaReplyAndCompensation,ISagaStepAndReply,ISagaStepAndCompensation
        where TSagaState: class
    {
        public SagaStepDefinition<TSagaState> PreviousStep { get; private set; }
        public SagaStepDefinition<TSagaState> NextStep { get; private set; }
        
        public SagaStepDefinition()
        {
        }
        public SagaStepDefinition(string name)
        {
            Name = name;
        }

        public ISagaParticipant Step(string name)
        {
            if (this.Name == null)
            {
                this.Name = name;
                return this;
            }
            var newStep = new SagaStepDefinition<TSagaState>(name);
            SagaStepDefinition<TSagaState> leafNode = null;
            while ((leafNode = this).NextStep == null)
            {
                leafNode.NextStep = newStep;
                newStep.PreviousStep = leafNode;
            }

            return newStep;
        }

        public ISagaStepDefinition Build()
        {
            SagaStepDefinition<TSagaState> current = this;
            while (current.PreviousStep != null)
            {
                current = current.PreviousStep;
            }

            return current;
        }

        ISagaStepDefinition ISagaStepAndCompensation.AssignCompensation(Func<ISagaCommand,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedStage)
        {
            return AssignCompensation(handler,generatedStage);
        }

        ISagaStepDefinition ISagaStepAndReply.AssignReply(Func<ISagaReplyCommand> replyCommand, Action<ISagaReplyCommand> sagaStage)
        {
            return AssignReply(replyCommand,sagaStage);
        }


        public ISagaReplyAndCompensation AssignParticipant(Func<ISagaCommand,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedStage)
        {
            this.ParticipantHandler = handler;
            this.ParticipantStage = generatedStage;
            return this;
        }

        ISagaStepDefinition ISagaParticipant.AssignCompensation(Func<ISagaCommand,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedStage)
        {
            return AssignCompensation(handler, generatedStage);
        }

        public ISagaStepAndReply AssignCompensation(Func<ISagaCommand,SagaCommandEndpoint> handler, Func<ISagaCommand> generatedStage)
        {
            this.CompensationHandler = handler;
            this.CompensationStage = generatedStage;
            return this;
        }


        public ISagaStepAndCompensation AssignReply(Func<ISagaReplyCommand> replyCommand, Action<ISagaReplyCommand> sagaStage)
        {
            this.ReplyHandler = replyCommand;
            this.ReplyStage = sagaStage;
            return this;
        }
    }
}