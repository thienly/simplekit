using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SimpleKit.StateMachine.Definitions;

namespace SimpleKit.StateMachine
{
    public interface ISagaPersistence
    {
        SagaStateProxy Load(Guid sagaId);
        void Save(SagaStateProxy sagaState);
        void Save(List<SagaStateProxy> sagaStateProxies);
    }

    public class EmptySagaState : SagaStateProxy
    {
    }
    public class SagaPersistence : ISagaPersistence
    {
        private DbSet<SagaStateProxy> _stateProxies;

        public SagaPersistence(DbSet<SagaStateProxy> stateProxies)
        {
            _stateProxies = stateProxies;
        }

        public SagaStateProxy Load(Guid sagaId)
        {
            try
            {
                return _stateProxies.Where(x => x.SagaId == sagaId).OrderByDescending(x => x.Version).First();
            }
            catch (Exception e)
            {
                return new EmptySagaState();
            }
        }
        public void Save(SagaStateProxy sagaState)
        {
            _stateProxies.Add(sagaState);
        }

        public void Save(List<SagaStateProxy> sagaStateProxies)
        {
            _stateProxies.AddRange(sagaStateProxies);
        }
    }
}