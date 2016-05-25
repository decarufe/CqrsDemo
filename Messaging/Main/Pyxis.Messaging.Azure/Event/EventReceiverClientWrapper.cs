using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Pyxis.Messaging.Azure.Event
{
    public class EventReceiverClientWrapper : ClientWrapper<SubscriptionClient>
    {
        public EventReceiverClientWrapper(INodeIdentifier nodeIdentifier, IEventChannelDescriptor eventChannelDescriptor)
        {
            Initialize(nodeIdentifier, eventChannelDescriptor);
        }

        public override SubscriptionClient CreateClient(INodeIdentifier nodeIdentifier, IChannelDescriptor eventChannelDescriptor)
        {
            var channelName = ChannelNameUtils.GenerateName(nodeIdentifier.InstallName, eventChannelDescriptor.ChannelName);
            var subscriptionName = ChannelNameUtils.GenerateName(nodeIdentifier.InstallName, nodeIdentifier.NodeName, true);
            var nsManager = NamespaceManager.CreateFromConnectionString(eventChannelDescriptor.ConnectionString);
            if (!nsManager.SubscriptionExists(channelName, subscriptionName))
            {
                var description = new SubscriptionDescription(channelName, subscriptionName)
                {
                    DefaultMessageTimeToLive = TimeSpan.FromHours(1),
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(5),
                };
                nsManager.CreateSubscription(description);
            }

            // Initialize the connection to Service Bus Queue
            return SubscriptionClient.CreateFromConnectionString(eventChannelDescriptor.ConnectionString, channelName, subscriptionName);
        }
    }
}