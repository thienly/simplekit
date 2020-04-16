using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SimpleKit.StateMachine.Definitions;
using SimpleKit.StateMachine.Persistences;

namespace SimpleKit.StateMachine
{
    public interface ISagaManager<T> where T: class,ISagaState
    {
        SagaStateProxy Process(T state,SagaStateProxy sagaStateProxy);
    }

    public delegate object SagaStepDefinitionFactory(Type typem, object state);

    public interface ISagaPublisher
    {
        void Publish(SagaCommandContext context, byte[] data);
    }
    public class SagaManager<T> : ISagaManager<T> where T: class, ISagaState
    {
        private readonly ISagaPersistence _sagaPersistence;
        private readonly ISagaPublisher _sagaPublisher;
        public SagaManager(ISagaPersistence sagaPersistence, ISagaPublisher sagaPublisher)
        {
            _sagaPersistence = sagaPersistence;
            _sagaPublisher = sagaPublisher;
        }
        public SagaStateProxy Process(T sagaState, SagaStateProxy sagaStateProxy)
        {
            SagaStateProxy result;
            var types = Assembly.GetCallingAssembly().GetTypes()
                .FirstOrDefault(x => x.BaseType == typeof(SagaDefinition<>).MakeGenericType(sagaState.GetType()));
            var sagaDefinition = (SagaDefinition<T>) Activator.CreateInstance(types, new object[] {(T) sagaState});
            
            if (sagaStateProxy.IsCompleted)
                throw new SagaException(sagaState.SagaId, "The saga is already completed");
            // start from beginning 
            var stepDefinition = sagaDefinition.GetStepWithName(sagaStateProxy.NextState);
            // get direction
            // forward or backward.
            SagaDirection sagaDirection =
                sagaStateProxy is EmptySagaState ? SagaDirection.Forward : sagaStateProxy.Direction;
            if (!(sagaStateProxy is EmptySagaState))
            {
                sagaState = (T)sagaStateProxy.SagaState;
            }
            switch (sagaDirection)
            {
                case SagaDirection.Forward:
                    result = ProcessForward();
                    break;
                case SagaDirection.Backward:
                    result = ProcessBackward();
                    break;
                default:
                    throw new SagaException(sagaState.SagaId, "Saga direction is not correct");
            }

            return result;

            SagaStateProxy ProcessBackward()
            {
                var isCompleted = sagaDefinition.IsCompleted(stepDefinition.Name, SagaDirection.Backward);
                // Execute the participant.
                var command = stepDefinition.CompensationStage.Invoke();
                var definitionCompensationHandler = stepDefinition.CompensationHandler(command,sagaState);
                SagaStateProxy newProxy;
                if (definitionCompensationHandler is NoReplyCommandEndpoint)
                {
                    // what happen if publish message is failed
                    var version = sagaStateProxy.Version + 1;
                    newProxy = new SagaStateProxy(sagaState, (bool) isCompleted, DateTime.Now,
                        version, stepDefinition.Name,
                        stepDefinition.PreviousStep == null ? stepDefinition.Name : stepDefinition.PreviousStep.Name,
                        sagaDefinition.GetType());
                    newProxy.MoveBackward();
                }
                else
                {
                    var version = sagaStateProxy.Version + 1;
                    newProxy = new SagaStateProxy(sagaState, false, DateTime.Now, version,
                        stepDefinition.Name,
                        $"WaitingFor_{stepDefinition.Name}",
                        sagaDefinition.GetType());
                    newProxy.MoveBackward();
                }

                return newProxy;
            }

            // Handler must be handle all cases from retries or throw an exception. The engine only produce sagaProxy
            SagaStateProxy ProcessForward()
            {
                var isCompleted = sagaDefinition.IsCompleted(stepDefinition.Name, SagaDirection.Forward);
                SagaStateProxy newProxy;
                // Execute the participant.
                try
                {
                    var command = stepDefinition.ParticipantStage.Invoke();
                    var definitionParticipantHandler = stepDefinition.ParticipantHandler(command,sagaState);
                    if (definitionParticipantHandler is NoReplyCommandEndpoint)
                    {
                        // what happen if publish message is failed
                        var version = sagaStateProxy.Version + 1;
                        newProxy = new SagaStateProxy(sagaState, (bool) isCompleted, DateTime.Now,
                            version, stepDefinition.Name,
                            stepDefinition.NextStep == null ? stepDefinition.Name : stepDefinition.NextStep.Name,
                            sagaDefinition.GetType());
                        newProxy.MoveForward();
                    }
                    else
                    {
                        // send the message, getting confirmation from message broker and then saving to the database.
                        _sagaPublisher.Publish(definitionParticipantHandler.SagaCommandContext, definitionParticipantHandler.Data);
                        var version = sagaStateProxy.Version + 1;
                        newProxy = new SagaStateProxy(sagaState, isCompleted, DateTime.Now, version,
                            stepDefinition.Name,
                            $"WaitingFor_{stepDefinition.Name}",
                            sagaDefinition.GetType());
                        newProxy.MoveForward();
                    }
                }
                catch (Exception e)
                {
                    var version = sagaStateProxy.Version + 1;
                    newProxy = new SagaStateProxy(sagaState, false, DateTime.Now,
                        version, stepDefinition.Name,
                        stepDefinition.PreviousStep == null ? stepDefinition.Name : stepDefinition.PreviousStep.Name,
                        sagaDefinition.GetType());
                    newProxy.Error = new SagaException(newProxy.SagaId, e.Message);
                    newProxy.MoveBackward();
                }
                return newProxy;
            }
        }
    }
}