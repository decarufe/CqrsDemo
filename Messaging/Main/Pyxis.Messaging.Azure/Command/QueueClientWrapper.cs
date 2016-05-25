using System;
using log4net;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Pyxis.Messaging.Azure.Command
{
    public class QueueClientWrapper : ClientWrapper<QueueClient>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof (QueueClientWrapper));

        public QueueClientWrapper(INodeIdentifier nodeIdentifier, IChannelDescriptor channelDescriptor)
        {
            Initialize(nodeIdentifier, channelDescriptor);
        }

        public override QueueClient CreateClient(INodeIdentifier nodeIdentifier, IChannelDescriptor channelDescriptor)
        {
            var queueName = ChannelNameUtils.GenerateName(nodeIdentifier.InstallName, channelDescriptor.ChannelName);
            _logger.DebugFormat("Creating client for queue {0}", queueName);
            var nsManager = NamespaceManager.CreateFromConnectionString(channelDescriptor.ConnectionString);
            if (!nsManager.QueueExists(queueName))
            {
                _logger.DebugFormat("Creating queue does not exists, creating");
                var queueDescription = new QueueDescription(queueName);
                queueDescription.RequiresDuplicateDetection = channelDescriptor is ICommandChannelDescriptor ?
                    ((ICommandChannelDescriptor)channelDescriptor).DetectDuplicateMessages : true;
                nsManager.CreateQueue(queueDescription);
            }
            var exisistingQueue = nsManager.GetQueue(queueName);
            if (exisistingQueue != null)
            {
                _logger.DebugFormat("Queue requires duplicate detection: {0}", exisistingQueue.RequiresDuplicateDetection);
            }
            // Initialize the connection to Service Bus Queue
            return QueueClient.CreateFromConnectionString(channelDescriptor.ConnectionString, queueName);
        }
    }
}