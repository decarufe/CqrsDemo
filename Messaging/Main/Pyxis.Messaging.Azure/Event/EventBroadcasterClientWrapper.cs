using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Pyxis.Messaging.Azure.Event
{
    public class EventBroadcasterClientWrapper : ClientWrapper<TopicClient>
    {
        public EventBroadcasterClientWrapper(INodeIdentifier nodeIdentifier, IEventChannelDescriptor eventChannelDescriptor)
        {
            Initialize(nodeIdentifier, eventChannelDescriptor);
        }

        public override TopicClient CreateClient(INodeIdentifier nodeIdentifier, IChannelDescriptor eventChannelDescriptor)
        {
            var connectionString = eventChannelDescriptor.ConnectionString;
            var channelName = ChannelNameUtils.GenerateName(nodeIdentifier.InstallName,eventChannelDescriptor.ChannelName);

            var nsManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!nsManager.TopicExists(channelName))
            {
                nsManager.CreateTopic(channelName);
            }

            // Initialize the connection to Service Bus Queue
            return TopicClient.CreateFromConnectionString(connectionString, channelName);
        }
    }
}