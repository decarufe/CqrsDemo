using System;
using Pyxis.Cqrs.Events;

namespace Pyxis.Cqrs.Tests.Domain.Events
{
    public class TestOtherMethodCalled : DomainEvent
    {
        public TestOtherMethodCalled(Guid id) : base(id)
        {
        }
    }
}