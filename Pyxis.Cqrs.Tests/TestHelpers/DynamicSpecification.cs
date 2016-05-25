using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Snapshots;

namespace Pyxis.Cqrs.Tests.TestHelpers
{
    [TestFixture]
    public abstract class DynamicSpecification
    {
        protected IDomainRepository DomainRepository { get; set; }
        protected ISession Session { get; set; }
        protected abstract IEnumerable<DomainEvent> Given();
        protected abstract dynamic When();
        protected abstract dynamic BuildHandler();

        protected Snapshot Snapshot { get; set; }
        protected IList<DomainEvent> EventDescriptors { get; set; }
        protected List<DomainEvent> PublishedEvents { get; set; }
        protected Exception Exception { get; set; }

        protected static Func<EquivalencyAssertionOptions<DomainEvent>, EquivalencyAssertionOptions<DomainEvent>> DefaultExclude
        {
            get { return options => options.Excluding(m => m.TimeStamp).Excluding(m => m.SessionInfo); }
        }

        [SetUp]
        public void TestInitialize()
        {
            var eventstorage = new SpecDomainEventStorage(Given().ToList());
            var snapshotstorage = new SpecSnapShotStorage(Snapshot);
            var eventpublisher = new SpecDomainEventPublisher();
            var snapshotStrategy = new DefaultSnapshotStrategy();
            DomainRepository = new SnapshotRepository(snapshotstorage, snapshotStrategy,
                new DomainRepository(eventstorage, eventpublisher), eventstorage);
            Session = new Session(DomainRepository);

            var handler = BuildHandler();
            try
            {
                var @when = When();
                if (@when != null) handler.Handle(@when);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Exception = e;
            }

            Snapshot = snapshotstorage.Snapshot;

            PublishedEvents = eventpublisher.PublishedEvents;
            EventDescriptors = eventstorage.Events;
        }

        protected IEnumerable<DomainEvent> PublishEvents(params DomainEvent[] events)
        {
            for (int i = 0; i < events.Length; i++)
            {
                events[i].Version = i + 1;
            }
            return events;
        }

        protected void ExpectEvents(params DomainEvent[] domainEvents)
        {
            PublishedEvents.ShouldAllBeEquivalentTo(domainEvents, DefaultExclude);
        }
    }
}