using CqrsDemo.Events;
using CqrsDemo.Services;
using Pyxis.Cqrs.Events;

namespace CqrsDemo.EventHandlers
{
    public class OrderEventHandler : 
        IHandleEvent<OrderCreated>,
        IHandleEvent<OrderLineAdded>
    {
        private OrderView _orderView;

        public OrderEventHandler()
        {
            _orderView = new OrderView();
        }

        public void Handle(OrderCreated message)
        {
            _orderView.Add(new OrderDto()
            {
                Id = message.Id
            });
        }

        public void Handle(OrderLineAdded message)
        {
            var orderDto = _orderView.GetByKey(message.Id);
            orderDto.OrderLines.Add(new OrderLineDto()
            {
                Quantity = message.OrderLine.Quantity,
                ProductName = message.OrderLine.ProductName
            });
        }
    }
}