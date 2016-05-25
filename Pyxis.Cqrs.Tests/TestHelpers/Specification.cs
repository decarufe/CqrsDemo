using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Domain.Exception;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Snapshots;

namespace Pyxis.Cqrs.Tests.TestHelpers
{
    [TestFixture]
    public abstract class Specification<TAggregate, THandler, TCommand>
        where TAggregate : AggregateRoot
        where THandler : class, IHandleCommand<TCommand>
        where TCommand : IDomainCommand
    {
        protected TAggregate Aggregate { get; set; }
        protected ISession Session { get; set; }
        protected abstract IEnumerable<DomainEvent> Given();
        protected abstract TCommand When();
        protected abstract THandler BuildHandler();

        protected Snapshot Snapshot { get; set; }
        protected IList<DomainEvent> EventDescriptors { get; set; }
        protected List<DomainEvent> PublishedEvents { get; set; }

        [SetUp]
        public void TestInitialize()
        {
            var eventstorage = new SpecDomainEventStorage(Given().ToList());
            var snapshotstorage = new SpecSnapShotStorage(Snapshot);
            var eventpublisher = new SpecDomainEventPublisher();
            var snapshotStrategy = new DefaultSnapshotStrategy();
            var repository = new SnapshotRepository(snapshotstorage, snapshotStrategy,
                new DomainRepository(eventstorage, eventpublisher), eventstorage);
            Session = new Session(repository);

            try
            {
                Aggregate = Session.Get<TAggregate>(Guid.Empty);
            }
            catch (AggregateNotFoundException)
            {
            }

            var handler = BuildHandler();
            handler.Handle(When());

            Snapshot = snapshotstorage.Snapshot;

            PublishedEvents = eventpublisher.PublishedEvents;
            EventDescriptors = eventstorage.Events;
        }
    }

    internal class SpecSnapShotStorage : ISnapshotStore
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

    internal class SpecDomainEventPublisher : IDomainEventPublisher
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

    internal class SpecDomainEventStorage : IDomainEventStore
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
            return Events.Where(x => x.Id == aggregateId && x.Version > fromVersion);
        }
    }
}