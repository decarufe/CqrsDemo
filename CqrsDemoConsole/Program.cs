using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CqrsDemo.CommandHandlers;
using CqrsDemo.Commands;
using CqrsDemo.Domain;
using CqrsDemo.EventHandlers;
using CqrsDemo.Services;
using Pyxis.Cqrs.Bus;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Messages;
using Pyxis.Cqrs.Result;
using Pyxis.Messaging;
using Pyxis.Messaging.Command;
using Pyxis.Persistance;

namespace CqrsDemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setting-up the bus
            OrderView orderView = new OrderView();
            var bus = SetupBus(orderView);
            
            // Sending commands
            var id1 = Guid.NewGuid();
            bus.SendAndWait(new CreateOrder(id1));
            bus.SendAndWait(new AddOrderLine(id1, new OrderLine(5, "My Product")));
            bus.SendAndWait(new AddOrderLine(id1, new OrderLine(10, "My Other Product")));

            #region Other example

            //var id2 = Guid.NewGuid();
            //bus.SendAndWait(new CreateOrder(id2));
            //bus.SendAndWait(new AddOrderLine(id2, new OrderLine(1, "My Last Product")));

            #endregion

            // Displaying the view
            foreach (var orderDto in orderView.GetAll())
            {
                Console.WriteLine(orderDto);
            }
        }

        #region Setup

        private static MessagingBus SetupBus(OrderView orderView)
        {
            IPersistanceStore persitenceStore = new MemoryPersistance();
            IDomainEventQuery domainEventQuery = new InProcPersistedDomainEventQuery(persitenceStore);
            IDomainEventStore domainEventStore = new PyxisDomainEventStore(
                persitenceStore, 
                domainEventQuery);
            IDomainResultStoreQuery storeQuery = new InProcDomainResultStoreQuery(persitenceStore);
            IDomainResultStore resultStore = new DomainResultPersistanceStore(
                persitenceStore,
                storeQuery);
            ITimeoutProvider timeoutProvider = new TimeoutProvider(1000);
            IResultPublisher resultPublisher = new ResultPublisher(
                resultStore, 
                timeoutProvider);
            IResultAwaiter resultAwaiter = new ResultAwaiter(
                resultStore, 
                timeoutProvider);
            IList<IHandleCommand> commandHanlders = new List<IHandleCommand>();
            IList<IHandleEvent> eventHandlers = new List<IHandleEvent>();
            IMessageDispatcher<ICommand> commandDispatcher = new CqrsMessagingHandler(
                resultPublisher, 
                commandHanlders,
                eventHandlers);
            ICommandQueue commandQueue = new InProcCommandQueue(commandDispatcher);
            MessagingBus bus = new MessagingBus(
                commandQueue, 
                resultPublisher, 
                resultAwaiter);
            IDomainRepository domainRepository = new DomainRepository(
                domainEventStore, 
                bus);
            ISession session = new Session(domainRepository);

            commandHanlders.Add(new OrderCommandHandler(session));
            eventHandlers.Add(new OrderEventHandler(orderView));

            return bus;
        }

        #endregion
    }
}
