using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.ServiceBus.Messaging;
using Pyxis.Messaging.Command;

namespace Pyxis.Messaging.Azure.Command
{
    public class CommandQueue : ICommandQueue, IDisposable
    {
        private readonly ICommandChannelDescriptor _channelDescriptor;
        private readonly INodeIdentifier _nodeIdentifier;
        private QueueClientWrapper _clientWrapper;
        private readonly ILog _logger = LogManager.GetLogger(typeof (CommandQueue));
        
        public CommandQueue(ICommandChannelDescriptor channelDescriptor, INodeIdentifier nodeIdentifier)
        {
            _channelDescriptor = channelDescriptor;
            _nodeIdentifier = nodeIdentifier;
            Reconnect();
        }

        public void Dispose()
        {
            _clientWrapper.Dispose();
        }

        public void QueueCommand(params ICommand[] commands)
        {
             QueueCommand(DateTime.MinValue, commands);
        }

        public void QueueCommand(DateTime time, params ICommand[] commands)
        {
            QueueCommand(true, time, commands);
        }

        private  void QueueCommand(bool firstTry, DateTime time, params ICommand[] commands)
        {
            try
            {
                var messages = new List<BrokeredMessage>();
                foreach (var command in commands)
                {
                    var brokeredMessage = BrokeredMessageTranslator.CreateMessage(command);
                    _logger.DebugFormat("Queuing message {0} with ID {1} {2} {3} to be executed ", command.Type.Value,
                        brokeredMessage.MessageId, command.Async ? "asynchronously" : "synchronously", 
                        time != DateTime.MinValue ? "at " + time : "now");
                    if (time != DateTime.MinValue)
                    {
                        brokeredMessage.ScheduledEnqueueTimeUtc = time.ToUniversalTime();
                    }
                    messages.Add(brokeredMessage);
                }
                
                _clientWrapper.Client.SendBatch(messages);
            }
            catch (Exception e)
            {
                _logger.Error("Error queuing command: ", e);
                if (firstTry)
                {
                    _logger.Error("Will retry");
                    Reconnect();
                    QueueCommand(false, time, commands);
                }
            }
        }

        public void QueueCommand(DateTime time, params CommandType[] types)
        {
            var commands = types.Select(x => CommandFactory.CreateCommand(x));
            QueueCommand(time, commands.ToArray());
        }

        public void QueueCommand(params CommandType[] types)
        {
            QueueCommand(DateTime.MinValue, types);
        }

        public void QueueFollowupCommand(CommandType type, ICommand sourceCommand)
        {
            QueueCommand(CommandFactory.CreateCommand(type, true, sourceCommand));
        }

        private void Reconnect()
        {
            if (_clientWrapper != null)
            {
                _clientWrapper.Dispose();                
            }

            _clientWrapper = new QueueClientWrapper(_nodeIdentifier, _channelDescriptor);
        }
    }
}