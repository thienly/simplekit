using System;
using System.Collections.Generic;

namespace SimpleKit.StateMachine.Persistences
{
    public interface ISagaPersistence
    {
        SagaStateProxy Load(Guid sagaId);
        void Save(SagaStateProxy sagaState);
        void Save(List<SagaStateProxy> sagaStateProxies);
    }
}