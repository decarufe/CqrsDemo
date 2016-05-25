using Pyxis.Cqrs.Events;
using System;

namespace Pyxis.Cqrs.Tests.Domain.Events
{
    public class TestEntityCreated : DomainEntityEvent
    {
        public TestEntityCreated(Guid id) : base(id)
        {
        }
    }
}