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
    public class AggregateMethodCallTest : AggregateSpecification<TestAggregate>
    {
        protected override IEnumerable<DomainEvent> Given()
        {
            return new List<DomainEvent>();
        }

        protected override TestAggregate When(ISession session, IDomainRepository domainRepository)
        {
            var aggregate = new TestAggregate(1.MakeGuid());
            aggregate.TestMethod();
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
            PublishedEvents.ShouldAllBeEquivalentTo(new DomainEvent[]
            {
                new TestAggregateCreated(1.MakeGuid())
                {
                    Version = 1
                },
                new TestMethodCalled(1.MakeGuid())
                {
                    Version = 2
                }
            }, options => options.Excluding(e => e.TimeStamp)
            .Excluding(e => e.SessionInfo));
        }
    }
}