using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using SimpleKit.StateMachine.Definitions;

namespace SimpleKit.StateMachine
{
    public interface ISagaPublisher
    {
        void Publish(SagaCommandEndpoint sagaCommandEndpoint);
    }

    public class SagaPublisher : ISagaPublisher
    {
        public void Publish(SagaCommandEndpoint sagaCommandEndpoint)
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = "10.0.19.103:9092",
                EnableIdempotence = true
            };

            var property = sagaCommandEndpoint.GetType()
                .GetProperty("Data", BindingFlags.Public | BindingFlags.Instance);
            var value = property.GetValue(sagaCommandEndpoint);
            using (var producer = new ProducerBuilder<Null, string>(config)
                .Build())
            {
                var topic = sagaCommandEndpoint.Channel;
                producer.Produce(topic, new Message<Null, string>()
                {
                    Timestamp = Timestamp.Default,
                    Value = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
                    })
                });
                producer.Flush();
            }
        }
    }

    public interface ISagaManager
    {
        void Process<TSagaState>(TSagaState state) where TSagaState : class, ISagaState;
        void ProcessOnReply(ISagaReplyCommand state);
    }

    public delegate object SagaStepDefinitionFactory(Type type);

    public class SagaManager : ISagaManager
    {
        private readonly SagaStepDefinitionFactory _factory;
        private readonly ISagaPersistence _sagaPersistence;
        private readonly ISagaPublisher _sagaPublisher;

        public SagaManager(SagaStepDefinitionFactory factory, ISagaPersistence sagaPersistence,
            ISagaPublisher sagaPublisher)
        {
            _factory = factory;
            _sagaPersistence = sagaPersistence;
            _sagaPublisher = sagaPublisher;
        }

        private SagaStepDefinition<TSagaState> GetStepDefinition<TSagaState>(ISagaState state)
            where TSagaState : class, ISagaState
        {
            var definition = typeof(SagaDefinition<>).MakeGenericType(state.GetType());
            var builder = _factory(definition);
            return (SagaStepDefinition<TSagaState>) definition.GetMethod("GetStepDefinition").Invoke(builder, null);
        }

        private Type GetDeclaringType<TSagaState>(TSagaState state) where TSagaState : class, ISagaState
        {
            var definition = typeof(SagaDefinition<>).MakeGenericType(state.GetType());
            var builder = _factory(definition);
            return builder.GetType();
        }

        public void Process<TSagaState>(TSagaState sagaState) where TSagaState : class, ISagaState
        {
            var sagaDefinition = (SagaDefinition<TSagaState>) _factory(typeof(SagaDefinition<TSagaState>));
            var sagaStateProxy = _sagaPersistence.Load(sagaState.SagaId);
            var stepDefinition = GetStepDefinition<TSagaState>(sagaState);
            if (sagaStateProxy is EmptySagaState)
            {
                // next step would be the first step.
                // forward step.
                // Determine the next state
                var listOfSagaProxy = new List<SagaStateProxy>();
                while (stepDefinition != null)
                {
                    var isCompleted = sagaDefinition.IsCompleted(stepDefinition.Name, SagaDirection.Forward);

                    // Execute the participant.
                    var command = stepDefinition.ParticipantStage.Invoke();
                    var definitionParticipantHandler = stepDefinition.ParticipantHandler(command);
                    if (definitionParticipantHandler is NoReplyCommandEndpoint)
                    {
                        // what happen if publish message is failed
                        var newSagaProxy = new SagaStateProxy(sagaState, (bool) isCompleted, DateTime.Now,
                            listOfSagaProxy.Count, stepDefinition.Name,
                            stepDefinition.NextStep.Name,
                            GetDeclaringType(sagaState));
                        newSagaProxy.MoveForward();
                        listOfSagaProxy.Add(newSagaProxy);
                    }
                    else
                    {
                        var newSagaProxy = new SagaStateProxy(sagaState, false, DateTime.Now, listOfSagaProxy.Count,
                            stepDefinition.Name,
                            $"WaitingFor_{stepDefinition.Name}",
                            GetDeclaringType(sagaState));
                        listOfSagaProxy.Add(newSagaProxy);
                        newSagaProxy.MoveForward();
                        _sagaPublisher.Publish(definitionParticipantHandler);
                    }

                    stepDefinition = stepDefinition.NextStep;
                }

                _sagaPersistence.Save(listOfSagaProxy);
            }
            else
            {
                
                if (sagaStateProxy.IsCompleted)
                    throw new SagaException(sagaStateProxy.SagaId, "Saga is already completed");

                var nextState = sagaStateProxy.NextState;

                var stepWithName = sagaDefinition.GetStepWithName(nextState);
                var listOfSagaProxy = new List<SagaStateProxy>();
                while (stepWithName != null)
                {
                    var isCompleted = sagaDefinition.IsCompleted(stepWithName.Name, SagaDirection.Forward);

                    if (sagaStateProxy.Direction == SagaDirection.Forward)
                    {
                        var command = stepDefinition.ParticipantStage.Invoke();
                        var definitionParticipantHandler = stepDefinition.ParticipantHandler(command);
                        if (definitionParticipantHandler is NoReplyCommandEndpoint)
                        {
                            var addProxy = sagaStateProxy.Clone();
                            addProxy.MoveForward();
                            addProxy.IncreaseVersion();
                            addProxy.IsCompleted = isCompleted;
                            if (isCompleted)
                            {
                                addProxy.MarkCompletedWithState(stepWithName.Name);
                            }
                            else
                            {
                                addProxy.SetNextState(stepWithName.Name);
                                addProxy.SetCurrentState(stepWithName.NextStep.Name);
                            }

                            listOfSagaProxy.Add(addProxy);
                        }
                        else
                        {
                            var addProxy = sagaStateProxy.Clone();
                            addProxy.MoveForward();
                            addProxy.IncreaseVersion();
                            addProxy.IsCompleted = isCompleted;
                            if (isCompleted)
                            {
                                addProxy.MarkCompletedWithState(stepWithName.Name);
                            }
                            else
                            {
                                addProxy.WaitingForState(stepWithName.Name);
                            }

                            listOfSagaProxy.Add(addProxy);

                            _sagaPublisher.Publish(definitionParticipantHandler);
                        }

                        stepWithName = stepWithName.NextStep;
                    }
                    else
                    {
                        var command = stepDefinition.CompensationStage.Invoke();
                        var definitionCompensationHandler = stepDefinition.CompensationHandler(command);
                        if (definitionCompensationHandler is NoReplyCommandEndpoint)
                        {
                            var addProxy = sagaStateProxy.Clone();
                            addProxy.MoveBackward();
                            addProxy.IncreaseVersion();
                            addProxy.IsCompleted = isCompleted;
                            if (isCompleted)
                            {
                                addProxy.MarkCompletedWithState(stepWithName.Name);
                            }
                            else
                            {
                                addProxy.SetNextState(stepWithName.PreviousStep.Name);
                                addProxy.SetCurrentState(stepWithName.Name);
                            }

                            listOfSagaProxy.Add(addProxy);
                        }
                        else
                        {
                            var addProxy = sagaStateProxy.Clone();
                            addProxy.MoveForward();
                            addProxy.IncreaseVersion();
                            addProxy.IsCompleted = isCompleted;
                            if (isCompleted)
                            {
                                addProxy.MarkCompletedWithState(stepWithName.Name);
                            }
                            else
                            {
                                addProxy.WaitingForState(stepWithName.Name);
                            }
                            listOfSagaProxy.Add(addProxy);

                            _sagaPublisher.Publish(definitionCompensationHandler);
                        }
                        stepWithName = stepWithName.PreviousStep;
                    }
                }

                _sagaPersistence.Save(listOfSagaProxy);
            }
        }

        public void ProcessOnReply(ISagaReplyCommand sagaReply)
        {
            var sagaStateProxy = _sagaPersistence.Load(sagaReply.SagaId);
            // Apply the change to the state
            var sagaState = sagaStateProxy.SagaState;
            var methodInfos = sagaState.GetType().GetMethods().ToList();
            var methodInfo = methodInfos.Single(x =>
                x.GetParameters().Length == 1 &&
                typeof(ISagaReplyCommand).IsAssignableFrom(x.GetParameters()[0].ParameterType));
            methodInfo.Invoke(sagaState, new[] {sagaReply});
            // Get the definition
            var sagaDefinition = _factory(sagaStateProxy.SagaDefinitionType);
            if (sagaReply.IsSuccess)
            {
                var method =
                    sagaStateProxy.SagaDefinitionType.GetMethod("GetNextStepName",
                        BindingFlags.Instance | BindingFlags.Public);
                var nextState = method.Invoke(sagaDefinition, new[] {sagaStateProxy.CurrentState});

                var isCompletedmethod =
                    sagaStateProxy.SagaDefinitionType.GetMethod("IsCompleted",
                        BindingFlags.Instance | BindingFlags.Public);
                var isCompleted = isCompletedmethod.Invoke(sagaDefinition,
                    new object[] {sagaStateProxy.CurrentState, SagaDirection.Forward});
                if ((bool) isCompleted)
                {
                    sagaStateProxy.MarkCompleted();
                    sagaStateProxy.SetNextState(nextState.ToString());
                }
                else
                {
                    sagaStateProxy.MarkUncompleted();
                    sagaStateProxy.SetNextState(nextState.ToString());
                    sagaStateProxy.MoveForward();
                }
            }
            else
            {
                var method =
                    sagaStateProxy.SagaDefinitionType.GetMethod("GetPreviousStepName",
                        BindingFlags.Instance | BindingFlags.Public);
                var previouseState = method.Invoke(sagaDefinition, new[] {sagaStateProxy.CurrentState});
                var isCompletedmethod =
                    sagaStateProxy.SagaDefinitionType.GetMethod("IsCompleted",
                        BindingFlags.Instance | BindingFlags.Public);
                var isCompleted = isCompletedmethod.Invoke(sagaDefinition,
                    new object[] {sagaStateProxy.CurrentState, SagaDirection.Backward});
                if ((bool) isCompleted)
                {
                    sagaStateProxy.MarkCompleted();
                }
                else
                {
                    sagaStateProxy.MarkUncompleted();
                    sagaStateProxy.SetNextState(previouseState.ToString());
                    sagaStateProxy.MoveBackward();
                }
            }

            sagaStateProxy.IncreaseVersion();
            _sagaPersistence.Save(sagaStateProxy);
        }
    }
}