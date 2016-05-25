using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Tests.Domain.Events;
using Pyxis.Cqrs.Tests.Domain.Objects;

namespace Pyxis.Cqrs.Tests.Domain
{
    [TestFixture]
    public class EntityTest : AggregateSpecification<TestAggregate>
    {
        protected override IEnumerable<DomainEvent> Given()
        {
            return new List<DomainEvent>()
            {
                new TestAggregateCreated(1.MakeGuid()) { Version = 1, TimeStamp = DateTimeOffset.Now}
            };
        }

        protected override TestAggregate When(ISession session, IDomainRepository domainRepository)
        {
            var aggregate = domainRepository.Get<TestAggregate>(GetGuid(1));
            int entityId = aggregate.CreateEntity<TestEntity>();
            return aggregate;
        }

        [Then]
        public void Should_publish_event()
        {
            PublishedEvents.Count.Should().Be(1);
        }

        [Then]
        public void Event_should_be_TestEvent()
        {
            PublishedEvents.First().Should().BeOfType<TestEntityCreated>();
        }
    }
}