using System;
using Pyxis.Cqrs.Events;

namespace Pyxis.Cqrs.Tests.Domain.Events
{
    public class TestOtherAggregateCreated : DomainEvent
    {
        public TestOtherAggregateCreated(Guid id) : base(id)
        {
        }
    }
}