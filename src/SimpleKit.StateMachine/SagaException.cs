using System;

namespace SimpleKit.StateMachine
{
    public class SagaException : Exception
    {
        public SagaException(Guid sagaId, string message): base(message)
        {
            SagaId = sagaId;
        }
        public Guid SagaId { get; set; }
        
    }
}