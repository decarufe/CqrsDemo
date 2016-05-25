using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Tests.Domain.Events;
using Pyxis.Cqrs.Tests.Domain.Objects;

namespace Pyxis.Cqrs.Tests.Domain
{
    [TestFixture]
    public class AggregateInterractionTest : AggregateSpecification<TestAggregate>
    {
        protected override IEnumerable<DomainEvent> Given()
        {
            return new List<DomainEvent>();
        }

        protected override TestAggregate When(ISession session, IDomainRepository domainRepository)
        {
            var aggregate = new TestAggregate(GetGuid(1));
            aggregate.TestInteractWithOtherAggregate(GetGuid(2), session);
            return aggregate;
        }

        [Then]
        public void Should_publish_event()
        {
            PublishedEvents.Count.Should().Be(2);
        }

        [Then]
        public void Event_should_be_TestEvent()
        {
            var info = new SessionInfo();
            PublishedEvents.ShouldAllBeEquivalentTo(new DomainEvent[]
            {
                new TestAggregateCreated(Guid.NewGuid())
                {
                    Id = GetGuid(1), SessionInfo = info, Version = 1
                },
                new TestOtherAggregateCreated(Guid.NewGuid()) 
                {
                    Id = GetGuid(2), SessionInfo = info, Version = 1
                }
            }, options => options.Excluding(e => e.TimeStamp));
        }
    }
}