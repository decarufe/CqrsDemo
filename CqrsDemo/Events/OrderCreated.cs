using System;
using Pyxis.Cqrs.Events;

namespace CqrsDemo.Events
{
    [Serializable]
    public class OrderCreated : DomainEvent
    {
        public OrderCreated(Guid id) : base(id)
        {
        }
    }
}