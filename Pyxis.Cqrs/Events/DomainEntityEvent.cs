using System;

namespace Pyxis.Cqrs.Events
{
    public class DomainEntityEvent : DomainEvent
    {
        public int EntityId { get; set; }

        public DomainEntityEvent(Guid id) : base(id)
        {
        }
    }
}