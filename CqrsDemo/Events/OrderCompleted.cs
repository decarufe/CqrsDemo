using System;
using Pyxis.Cqrs.Events;

namespace CqrsDemo.Events
{
    public class OrderCompleted : DomainEvent
    {
        public OrderCompleted(Guid id) : base(id)
        {
        }
    }
}