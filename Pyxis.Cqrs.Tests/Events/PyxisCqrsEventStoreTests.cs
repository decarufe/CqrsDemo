using System;
using System.Linq;
using NUnit.Framework;
using Pyxis.Cqrs.Events;
using Pyxis.Persistance;

namespace Pyxis.Cqrs.Tests.Events
{
    [TestFixture]
    public class PyxisCqrsEventStoreTests
    {
        private PyxisDomainEventStore _pyxisDomainEventStore;
        private Guid _aggregateId = Guid.NewGuid();
        [SetUp]
        public void SetUp()
        {
            var persistanceStore = new MemoryPersistance();
            _pyxisDomainEventStore = new PyxisDomainEventStore(persistanceStore, new InProcPersistedDomainEventQuery(persistanceStore));
        }

        [Test]
        public void EventsArePersisted()
        {
            var eventToStore = new TestEvent(_aggregateId) {Version = 1, TimeStamp = DateTime.Today};
            var aggregates = _pyxisDomainEventStore.Get(_aggregateId, 0);
            Assert.AreEqual(0, aggregates.Count());
            _pyxisDomainEventStore.Save(new [] { eventToStore}, new SessionInfo());
            aggregates = _pyxisDomainEventStore.Get(_aggregateId, 0);
            Assert.AreEqual(1, aggregates.Count());
            Assert.AreEqual(1, aggregates.First().Version);
            Assert.AreEqual(_aggregateId, aggregates.First().Id);
            Assert.AreEqual(DateTime.Today.ToString("G"), aggregates.First().TimeStamp.ToString("G"));
        }

        [Test]
        public void EventsCannotEraseExistingOne()
        {
            var eventToStore = new TestEvent(_aggregateId) { Version = 1, TimeStamp = DateTime.Today };
            var aggregates = _pyxisDomainEventStore.Get(_aggregateId, 0);
            Assert.AreEqual(0, aggregates.Count());
            _pyxisDomainEventStore.Save(new[] { eventToStore }, new SessionInfo());
            try
            {
                // Second one should throw
                _pyxisDomainEventStore.Save(new[] { eventToStore }, new SessionInfo());
                Assert.Fail("Should now allow to save event with existing ID");
            }
            catch (Exception)
            {
                // All good                
            }
        }

        [Test]
        public void EventsAreReturnedInSequence()
        {
            for (var count = 1; count < 3; count++)
            {
                var eventToStore = new TestEvent(_aggregateId) { Version = count, TimeStamp = DateTime.Today.AddDays(count) };
                _pyxisDomainEventStore.Save(new[] { eventToStore }, new SessionInfo());
            }
            

            var aggregates = _pyxisDomainEventStore.Get(_aggregateId, 0);
            Assert.AreEqual(2, aggregates.Count());
            Assert.AreEqual(1, aggregates.First().Version);
            Assert.AreEqual(2, aggregates.Last().Version);
            aggregates = _pyxisDomainEventStore.Get(_aggregateId, 2);
            Assert.AreEqual(1, aggregates.Count());
            Assert.AreEqual(2, aggregates.Last().Version);
        }

        [Test]
        public void EventsAreReturnedFromRequestedVersion()
        {
            for (var count = 1; count < 3; count++)
            {
                var eventToStore = new TestEvent(_aggregateId) { Version = count, TimeStamp = DateTime.Today.AddDays(count) };
                _pyxisDomainEventStore.Save(new[] { eventToStore }, new SessionInfo());
            }

            var aggregates = _pyxisDomainEventStore.Get(_aggregateId, 2);
            Assert.AreEqual(1, aggregates.Count());
            Assert.AreEqual(2, aggregates.Last().Version);
        }

        [Test]
        public void NothingIsReturnedWhenRequestedVersionIsTooHigh()
        {
            for (var count = 1; count < 3; count++)
            {
                var eventToStore = new TestEvent(_aggregateId) { Version = count, TimeStamp = DateTime.Today.AddDays(count) };
                _pyxisDomainEventStore.Save(new[] { eventToStore }, new SessionInfo());
            }

            var aggregates = _pyxisDomainEventStore.Get(_aggregateId, 3);
            Assert.AreEqual(0, aggregates.Count());
        }


        [Serializable]
        class TestEvent : DomainEvent
        {
            public TestEvent(Guid id) : base(id)
            {
            }
        }
    }
    
}
