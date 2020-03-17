using System;
using SimpleKit.StateMachine.Definitions;

namespace SimpleKit.StateMachine
{
    public class SagaStateProxy
    {
        public Guid Id { get; set; }
        public Guid SagaId { get;  set; }

        public SagaStateProxy()
        {
            
        }

        public SagaStateProxy(ISagaState internalState, bool isCompleted, DateTime createdDate, int version,
            string currentState, string nextState, Type sagaDefinitionType)
        {
            SagaId = internalState.SagaId;
            SagaState = internalState;
            IsCompleted = isCompleted;
            CreatedDate = createdDate;
            Version = version;
            CurrentState = currentState;
            NextState = nextState;
            SagaDefinitionType = sagaDefinitionType;
        }

        public ISagaState SagaState { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public bool IsCompleted { get;  set; }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }

        public void MarkCompletedWithState(string state)
        {
            this.CurrentState = state;
            this.NextState = state;
            IsCompleted = true;
        }

        public void MarkUncompleted()
        {
            IsCompleted = false;
        }
        public int Version { get; private set; }

        public void IncreaseVersion()
        {
            Version++;
        }
        public string CurrentState { get; private set; }

        public void SetCurrentState(string currentState)
        {
            CurrentState = currentState;
        }

        public void WaitingForState(string step)
        {
            this.CurrentState = step;
            this.NextState = $"WaitingFor_{step}";
        }
        public string NextState { get; private set; }

        public void SetNextState(string nextState)
        {
            NextState = nextState;
        }
        
        public Type SagaDefinitionType { get; private set; }

        public SagaStateProxy Clone()
        {
            return new SagaStateProxy()
            {
                SagaId = this.SagaId,
                SagaState = this.SagaState,
                IsCompleted = this.IsCompleted,
                CreatedDate = this.CreatedDate,
                Version = this.Version,
                CurrentState = this.CurrentState,
                NextState = this.NextState,
                SagaDefinitionType = this.SagaDefinitionType
            };
        }

        public SagaException Error { get; set; }
        public SagaDirection Direction { get; private set; } = SagaDirection.Forward;

        public void MoveForward()
        {
            Direction = SagaDirection.Forward;
        }

        public void MoveBackward()
        {
            Direction = SagaDirection.Backward;
        }
    }
    public enum SagaDirection
    {
        Backward,
        Forward
    }
}