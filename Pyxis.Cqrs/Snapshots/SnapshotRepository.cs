using System;
using System.Linq;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Domain.Factories;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Infrastructure;

namespace Pyxis.Cqrs.Snapshots
{
    public class SnapshotRepository : IDomainRepository
    {
        private readonly ISnapshotStore _snapshotStore;
        private readonly ISnapshotStrategy _snapshotStrategy;
        private readonly IDomainRepository _domainRepository;
        private readonly IDomainEventStore _domainEventStore;

        public SnapshotRepository(ISnapshotStore snapshotStore, ISnapshotStrategy snapshotStrategy,
            IDomainRepository domainRepository, IDomainEventStore domainEventStore)
        {
            if (snapshotStore == null)
                throw new ArgumentNullException("snapshotStore");
            if (snapshotStrategy == null)       
                throw new ArgumentNullException("snapshotStrategy");
            if (domainRepository == null)             
                throw new ArgumentNullException("domainRepository");
            if (domainEventStore == null)             
                throw new ArgumentNullException("domainEventStore");

            _snapshotStore = snapshotStore;
            _snapshotStrategy = snapshotStrategy;
            _domainRepository = domainRepository;
            _domainEventStore = domainEventStore;
        }

        public void Save<T>(T aggregate, SessionInfo sessionInfo, int? exectedVersion = null) where T : AggregateRoot
        {
            TryMakeSnapshot(aggregate);
            _domainRepository.Save(aggregate, sessionInfo, exectedVersion);
        }

        public T Get<T>(Guid aggregateId) where T : AggregateRoot
        {
            var aggregate = AggregateFactory.CreateAggregate<T>();
            var snapshotVersion = TryRestoreAggregateFromSnapshot(aggregateId, aggregate);
            if (snapshotVersion == -1)
            {
                return _domainRepository.Get<T>(aggregateId);
            }
            var events = _domainEventStore.Get(aggregateId, snapshotVersion).Where(desc => desc.Version > snapshotVersion);
            aggregate.LoadFromHistory(events);

            return aggregate;
        }

        private int TryRestoreAggregateFromSnapshot<T>(Guid id, T aggregate)
        {
            var version = -1;
            if (_snapshotStrategy.IsSnapshotable(typeof (T)))
            {
                var snapshot = _snapshotStore.Get(id);
                if (snapshot != null)
                {
                    aggregate.AsDynamic().Restore(snapshot);
                    version = snapshot.Version;
                }
            }
            return version;
        }

        private void TryMakeSnapshot(AggregateRoot aggregate)
        {
            if (!_snapshotStrategy.ShouldMakeSnapShot(aggregate))
                return;
            if (!_snapshotStrategy.IsSnapshotable(aggregate.GetType())) return;

            var snapshot = aggregate.AsDynamic().GetSnapshot().RealObject;
            snapshot.Version = aggregate.Version + aggregate.GetUncommittedChanges().Count();
            _snapshotStore.Save(snapshot);
        }
    }
}