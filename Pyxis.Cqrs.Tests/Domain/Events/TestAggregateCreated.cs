using System;
using Pyxis.Cqrs.Events;

namespace Pyxis.Cqrs.Tests.Domain.Events
{
    public class TestAggregateCreated : DomainEvent
    {
        public TestAggregateCreated(Guid id) : base(id)
        {
        }
    }
}