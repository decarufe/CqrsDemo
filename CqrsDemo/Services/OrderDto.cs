using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CqrsDemo.Services
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public IList<OrderLineDto> OrderLines { get; set; }

        public OrderDto()
        {
            OrderLines = new List<OrderLineDto>();
        }

        public override string ToString()
        {
            return string.Format("Id: {0} \n{1}", Id, string.Join("\n", OrderLines));
        }
    }
}