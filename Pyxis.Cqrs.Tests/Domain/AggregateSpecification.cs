using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Domain.Exception;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Snapshots;

namespace Pyxis.Cqrs.Tests.Domain
{
    [TestFixture]
    public abstract class AggregateSpecification<TAggregate> where TAggregate : AggregateRoot
    {
        protected ISession Session { get; set; }
        protected SessionInfo sessionInfo { get; set; }
        protected abstract IEnumerable<DomainEvent> Given();
        protected abstract TAggregate When(ISession session, IDomainRepository domainRepository);

        protected Snapshot Snapshot { get; set; }
        protected IList<DomainEvent> EventDescriptors { get; set; }
        protected IList<DomainEvent> PublishedEvents { get; set; }

        private IDictionary<int, Guid> _generatedGuids = new Dictionary<int, Guid>();

        [SetUp]
        public void TestInitialize()
        {
            sessionInfo = new SessionInfo();
            var eventstorage = new SpecDomainEventStorage(Given().ToList());
            var eventpublisher = new SpecDomainEventPublisher();
            var snapshotstorage = GetSnapshotstorage();
            var snapshotStrategy = GetSnapshotStrategy();
            var repository = new SnapshotRepository(snapshotstorage, snapshotStrategy,
                new DomainRepository(eventstorage, eventpublisher), eventstorage);

            Session = new Session(repository);
            Session.Add(When(Session, repository));
            Session.Commit(sessionInfo);

            PublishedEvents = eventpublisher.PublishedEvents;
            EventDescriptors = eventstorage.Events;
        }

        protected virtual ISnapshotStore GetSnapshotstorage()
        {
            return new SpecSnapShotStorage(Snapshot);
        }

        protected virtual ISnapshotStrategy GetSnapshotStrategy()
        {
            return new NoSnapshotStrategy();
        }

        private class SpecSnapShotStorage : ISnapshotStore
        {
            public SpecSnapShotStorage(Snapshot snapshot)
            {
                Snapshot = snapshot;
            }

            public Snapshot Snapshot { get; set; }

            public Snapshot Get(Guid id)
            {
                return Snapshot;
            }

            public void Save(Snapshot snapshot)
            {
                Snapshot = snapshot;
            }
        }

        private class SpecDomainEventPublisher : IDomainEventPublisher
        {
            public SpecDomainEventPublisher()
            {
                PublishedEvents = new List<DomainEvent>();
            }

            public void Publish<T>(T[] events, string trackingId = null) where T : DomainEvent
            {
                PublishedEvents.AddRange(events);
            }

            public List<DomainEvent> PublishedEvents { get; set; }
        }

        private class SpecDomainEventStorage : IDomainEventStore
        {
            public SpecDomainEventStorage(List<DomainEvent> events)
            {
                Events = events;
            }

            public List<DomainEvent> Events { get; set; }

            public void Save(IEnumerable<DomainEvent> events, SessionInfo sessionInfo)
            {
                foreach (var cqrsEvent in events)
                {
                    cqrsEvent.SessionInfo = sessionInfo;
                    Events.Add(cqrsEvent);
                }
            }

            public IEnumerable<DomainEvent> Get(Guid aggregateId, int fromVersion)
            {
                return Events.Where(x => x.Version > fromVersion);
            }
        }

        protected Guid GetGuid(int index)
        {
            if (!_generatedGuids.ContainsKey(index))
            {
                _generatedGuids.Add(index, Guid.NewGuid());
            }

            return _generatedGuids[index];
        }
    }
}