using System;
using System.Diagnostics;
using Microsoft.ServiceBus.Messaging;
using Pyxis.Messaging.Command;

namespace Pyxis.Messaging.Azure.Command
{
    public class CommandListener : IMessageListener, IDisposable
    {
        private readonly INodeIdentifier _nodeIdentifier;
        private readonly IChannelDescriptor _channelDescriptor;
        private readonly IMessageDispatcher<ICommand> _commandDispatcher;
        private QueueClientWrapper _clientWrapper;

        public CommandListener(INodeIdentifier nodeIdentifier, ICommandChannelDescriptor channelDescriptor, IMessageDispatcher<ICommand> commandDispatcher)
        {
            _nodeIdentifier = nodeIdentifier;
            _channelDescriptor = channelDescriptor;
            _commandDispatcher = commandDispatcher;
        }

        private void DispatchMessage(BrokeredMessage receivedMessage)
        {
            try
            {
                var message = CommandFactory.CreateCommand(receivedMessage.MessageId,
                    receivedMessage.GetBody<string>(),
                    receivedMessage.Properties);
                Trace.TraceInformation("Received message {0}", message.Type.Value);
                var handled = _commandDispatcher.Dispatch(message);

                if (handled)
                {
                    receivedMessage.Complete();
                }
                else
                {
                    receivedMessage.Abandon();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                receivedMessage.DeadLetter();
            }
        }
        
        public void Dispose()
        {
            _clientWrapper.Close();
        }

        public void Listen()
        {
            _clientWrapper = new QueueClientWrapper(_nodeIdentifier, _channelDescriptor);
            _clientWrapper.Client.OnMessage(DispatchMessage);
        }

        public void Close()
        {
            _clientWrapper.Close();
        }
    }
}
