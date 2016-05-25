using System.Threading;
using log4net;
using Microsoft.ServiceBus.Messaging;
using Pyxis.Messaging.Command;
using Pyxis.Messaging.Events;

namespace Pyxis.Messaging.Azure.Event
{
    public class EventBus : IEventBus
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(EventBus)); 
        private readonly IMessageDispatcher<IEvent> _eventDispatcher;
        private readonly EventBroadcasterClientWrapper _broadcasterClientWrapper;
        private readonly EventReceiverClientWrapper _receiverClientWrapper;

        public EventBus(INodeIdentifier nodeIdentifier, IEventChannelDescriptor eventChannelDescriptor, IMessageDispatcher<IEvent> eventDispatcher)
        {
            _logger.Debug("Building bus!");
            _logger.Debug(string.Format("Event Channel is {0} using connection string {1}", eventChannelDescriptor.ChannelName,eventChannelDescriptor.ConnectionString));
            _logger.Debug(string.Format("Deployment {0} has node name {1}", nodeIdentifier.InstallName, nodeIdentifier.NodeName));
            _eventDispatcher = eventDispatcher;
            _logger.Debug("Creating broadcaster client");
            _broadcasterClientWrapper = new EventBroadcasterClientWrapper(nodeIdentifier, eventChannelDescriptor);
            _logger.Debug("Creating receiver client");
            _receiverClientWrapper = new EventReceiverClientWrapper(nodeIdentifier, eventChannelDescriptor );
            _logger.Debug("Registering event handler");
            _receiverClientWrapper.Client.OnMessage(DispatchEvent);
        }

        public void Broadcast(EventType eventType, string data = null)
        {
            _logger.Info(string.Format("Broadcasting message {0} with data {1}", eventType.Value, data));
            var eventObj = EventFactory.CreateEvent(eventType, data);
            Broadcast(eventObj);
        }

        public void Broadcast(IEvent eventObj)
        {
            var brokeredMessage = BrokeredMessageTranslator.CreateMessage(eventObj);
            _broadcasterClientWrapper.Client.Send(brokeredMessage);
        }

        private void DispatchEvent(BrokeredMessage receivedMessage)
        {
            var eventObj = EventFactory.CreateEvent(receivedMessage.GetBody<string>(), receivedMessage.Properties);
            _logger.Info(string.Format("Calling dispatcher with message {0} and data {1}", eventObj.Type.Value, eventObj.Content));
            var wcb = new WaitCallback(x => _eventDispatcher.Dispatch(x as IEvent));
            ThreadPool.QueueUserWorkItem(wcb, eventObj);
            receivedMessage.Complete();
        }

    }
}
