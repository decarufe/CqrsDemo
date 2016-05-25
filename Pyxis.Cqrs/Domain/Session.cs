using System;
using System.Collections.Generic;
using Pyxis.Cqrs.Domain.Exception;
using Pyxis.Cqrs.Events;

namespace Pyxis.Cqrs.Domain
{
    public class Session : ISession
    {
        private readonly IDomainRepository _domainRepository;
        private readonly Dictionary<Guid, AggregateDescriptor> _trackedAggregates;

        public Session(IDomainRepository domainRepository)
        {
            if (domainRepository == null)
                throw new ArgumentNullException("domainRepository");

            _domainRepository = domainRepository;
            _trackedAggregates = new Dictionary<Guid, AggregateDescriptor>();
        }

        public void Add<T>(T aggregate) where T : AggregateRoot
        {
            if (!IsTracked(aggregate.Id))
                _trackedAggregates.Add(aggregate.Id,
                    new AggregateDescriptor {Aggregate = aggregate, Version = aggregate.Version});
            else if (_trackedAggregates[aggregate.Id].Aggregate.Id != aggregate.Id)
                throw new ConcurrencyException(aggregate.Id);
        }

        public T Get<T>(Guid id, int? expectedVersion = null) where T : AggregateRoot
        {
            if (IsTracked(id))
            {
                var trackedAggregate = (T) _trackedAggregates[id].Aggregate;
                if (expectedVersion != null && trackedAggregate.Version != expectedVersion)
                    throw new ConcurrencyException(trackedAggregate.Id);
                return trackedAggregate;
            }

            var aggregate = _domainRepository.Get<T>(id);
            if (expectedVersion != null && aggregate.Version != expectedVersion)
                throw new ConcurrencyException(id);
            Add(aggregate);

            return aggregate;
        }

        private bool IsTracked(Guid id)
        {
            return _trackedAggregates.ContainsKey(id);
        }

        public void Commit(SessionInfo sessionInfo = null)
        {
            if (sessionInfo == null) sessionInfo = new NullSessionInfo();
            foreach (var descriptor in _trackedAggregates.Values)
            {
                // TODO: Put back versioning
                _domainRepository.Save(descriptor.Aggregate, sessionInfo/*, descriptor.Version*/);
            }
            _trackedAggregates.Clear();
        }

        private class AggregateDescriptor
        {
            public AggregateRoot Aggregate { get; set; }
            public int Version { get; set; }
        }
    }

    public class NullSessionInfo : SessionInfo
    {
    }
}