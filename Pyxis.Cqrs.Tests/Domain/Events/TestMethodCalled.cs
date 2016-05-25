using System;
using Pyxis.Cqrs.Events;

namespace Pyxis.Cqrs.Tests.Domain.Events
{
    public class TestMethodCalled : DomainEvent
    {
        public TestMethodCalled(Guid id) : base(id)
        {
        }
    }
}