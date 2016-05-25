using System;
using System.Linq;
using Pyxis.Cqrs.Domain.Exception;
using Pyxis.Cqrs.Domain.Factories;
using Pyxis.Cqrs.Events;

namespace Pyxis.Cqrs.Domain
{
    public class DomainRepository : IDomainRepository
    {
        private readonly IDomainEventStore _domainEventStore;
        private readonly IDomainEventPublisher _domainEventPublisher;
            
        public DomainRepository(IDomainEventStore domainEventStore, IDomainEventPublisher domainEventPublisher)
        {
            _domainEventStore = domainEventStore;
            _domainEventPublisher = domainEventPublisher;
        }

        public void Save<T>(T aggregate, SessionInfo sessionInfo, int? expectedVersion = null) where T : AggregateRoot
        {
            if (expectedVersion != null && _domainEventStore.Get(aggregate.Id, expectedVersion.Value).Any())
                throw new ConcurrencyException(aggregate.Id);

            var changes = aggregate.FlushUncommitedChanges().ToArray();
            if (changes.Any())
            {
                _domainEventStore.Save(changes, sessionInfo);
                _domainEventPublisher.Publish(changes, sessionInfo.TrackingId);
            }
        }

        public T Get<T>(Guid aggregateId) where T : AggregateRoot
        {
            return LoadAggregate<T>(aggregateId);
        }

        private T LoadAggregate<T>(Guid id) where T : AggregateRoot
        {
            var aggregate = AggregateFactory.CreateAggregate<T>();

            var events = _domainEventStore.Get(id, -1);
            if (!events.Any())
                throw new AggregateNotFoundException(id);

            aggregate.LoadFromHistory(events);
            return aggregate;
        }
    }
}