using System;
using System.Collections.Generic;

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
    }
}