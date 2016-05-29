using System;
using CqrsDemo.Domain;
using Pyxis.Cqrs.Events;

namespace CqrsDemo.Events
{
    [Serializable]
    public class OrderLineAdded : DomainEvent
    {
        private readonly OrderLine _orderLine;

        public OrderLineAdded(Guid id, OrderLine orderLine) : base(id)
        {
            _orderLine = orderLine;
        }

        public OrderLine OrderLine
        {
            get { return _orderLine; }
        }
    }
}