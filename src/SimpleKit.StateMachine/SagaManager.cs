using System;
using SimpleKit.StateMachine.Definitions;
using SimpleKit.StateMachine.Persistences;

namespace SimpleKit.StateMachine
{
    public interface ISagaManager
    {
        SagaStateProxy Process<TSagaState>(TSagaState state) where TSagaState : class, ISagaState;
    }

    public delegate object SagaStepDefinitionFactory(Type type);

    public class SagaManager : ISagaManager
    {
        private readonly SagaStepDefinitionFactory _factory;
        private readonly ISagaPersistence _sagaPersistence;

        public SagaManager(SagaStepDefinitionFactory factory, ISagaPersistence sagaPersistence )
        {
            _factory = factory;
            _sagaPersistence = sagaPersistence;
        }

        private Type GetDeclaringType<TSagaState>() where TSagaState : class, ISagaState
        {
            var definition = typeof(SagaDefinition<>).MakeGenericType(typeof(TSagaState));
            var builder = _factory(definition);
            return builder.GetType();
        }

        public SagaStateProxy Process<TSagaState>(TSagaState sagaState) where TSagaState : class, ISagaState
        {
            SagaStateProxy result;
            var sagaDefinition = (SagaDefinition<TSagaState>) _factory(typeof(SagaDefinition<TSagaState>));
            var sagaStateProxy = _sagaPersistence.Load(sagaState.SagaId);
            
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
                sagaState = (TSagaState) sagaStateProxy.SagaState;
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
                        GetDeclaringType<TSagaState>());
                    newProxy.MoveBackward();
                }
                else
                {
                    var version = sagaStateProxy.Version + 1;
                    newProxy = new SagaStateProxy(sagaState, false, DateTime.Now, version,
                        stepDefinition.Name,
                        $"WaitingFor_{stepDefinition.Name}",
                        GetDeclaringType<TSagaState>());
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
                            GetDeclaringType<TSagaState>());
                        newProxy.MoveForward();
                    }
                    else
                    {
                        var version = sagaStateProxy.Version + 1;
                        newProxy = new SagaStateProxy(sagaState, isCompleted, DateTime.Now, version,
                            stepDefinition.Name,
                            $"WaitingFor_{stepDefinition.Name}",
                            GetDeclaringType<TSagaState>());
                        newProxy.MoveForward();
                    }
                }
                catch (Exception e)
                {
                    var version = sagaStateProxy.Version + 1;
                    newProxy = new SagaStateProxy(sagaState, false, DateTime.Now,
                        version, stepDefinition.Name,
                        stepDefinition.PreviousStep == null ? stepDefinition.Name : stepDefinition.PreviousStep.Name,
                        GetDeclaringType<TSagaState>());
                    newProxy.Error = new SagaException(newProxy.SagaId, e.Message);
                    newProxy.MoveBackward();
                }
                return newProxy;
            }
        }
    }
}