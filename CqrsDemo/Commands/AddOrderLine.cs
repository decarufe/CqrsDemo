using System;
using CqrsDemo.Domain;
using Pyxis.Cqrs.Commands;

namespace CqrsDemo.Commands
{
    public class AddOrderLine : DomainCommand
    {
        private readonly OrderLine _orderLine;

        public AddOrderLine(Guid id, OrderLine orderLine) : base(id)
        {
            _orderLine = orderLine;
        }

        public OrderLine OrderLine
        {
            get { return _orderLine; }
        }
    }
}