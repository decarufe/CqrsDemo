using System;
using Pyxis.Cqrs.Events;

namespace Pyxis.Cqrs.Tests.Events
{
    class TestDomainEvent : DomainEvent
    {
        public TestDomainEvent(Guid id) : base(id)
        {
        }
    }
}
