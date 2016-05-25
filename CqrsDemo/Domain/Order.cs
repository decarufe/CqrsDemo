using System;
using CqrsDemo.Commands;
using CqrsDemo.Events;
using Pyxis.Cqrs.Domain;

namespace CqrsDemo.Domain
{
    public class Order : AggregateRoot
    {
        public Order()
        {
            // Used to rebuild aggregate
        }

        public Order(Guid id)
        {
            Id = id;
            ApplyChange(new OrderCreated(id));
        }

        public void AddLine(OrderLine orderLine)
        {
            ApplyChange(new OrderLineAdded(Id, orderLine));
        }

        public void Complete()
        {
            ApplyChange(new OrderCompleted(Id));
        }
    }
}