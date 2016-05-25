using CqrsDemo.Commands;
using CqrsDemo.Domain;
using CqrsDemo.Events;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Domain;

namespace CqrsDemo.CommandHandlers
{
    public class OrderCommandHandler : 
        IHandleCommand<CreateOrder>,
        IHandleCommand<AddOrderLine>,
        IHandleCommand<CompleteOrder>
    {
        private readonly ISession _session;

        public OrderCommandHandler(ISession session)
        {
            _session = session;
        }

        public void Handle(CreateOrder message)
        {
            var order = new Order(message.Id);
            _session.Add(order);
            _session.Commit();
        }


        public void Handle(AddOrderLine message)
        {
            var order = _session.Get<Order>(message.Id);
            order.AddLine(message.OrderLine);
            _session.Commit();
        }

        public void Handle(CompleteOrder message)
        {
            var order = _session.Get<Order>(message.Id);
            order.Complete();
            _session.Commit();
        }
    }
}