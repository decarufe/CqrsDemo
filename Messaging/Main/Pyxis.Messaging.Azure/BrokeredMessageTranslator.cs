using Microsoft.ServiceBus.Messaging;
using Pyxis.Messaging.Command;
using Pyxis.Messaging.Events;

namespace Pyxis.Messaging.Azure
{
    public class BrokeredMessageTranslator
    {
        private const string AsyncAttribute = "__async";

        public static BrokeredMessage CreateMessage(ICommand command)
        {
            return CreateForCommand(command);
        }

        public static BrokeredMessage CreateMessage(IEvent eventObj)
        {
            return CreateForEvent(eventObj);
        }

        public static ICommand CreateCommand(BrokeredMessage message)
        {
            var async = message.Properties.ContainsKey(AsyncAttribute);
            message.Properties.Remove(AsyncAttribute);
            var command = CommandFactory.CreateCommand(message.MessageId,
                   message.GetBody<string>(),
                   message.Properties, async);
            return command;
        }

        public static IEvent CreateEvent(BrokeredMessage receivedMessage)
        {
            return EventFactory.CreateEvent(receivedMessage.GetBody<string>(), receivedMessage.Properties);
        }
        
        private static BrokeredMessage CreateForEvent(IEvent @event)
        {
            var message = CreateGenericMessage(@event);
            return message;
        }

        private static BrokeredMessage CreateForCommand(ICommand command)
        {
            var message = CreateGenericMessage(command);
            if (command.Async)
            {
                message.Properties[AsyncAttribute] = string.Empty;
            }
            return message;
        }

        private static BrokeredMessage CreateGenericMessage(IMessage message) 
        {
            var brokeredMessage = new BrokeredMessage(message.Content);
            if (!string.IsNullOrEmpty(message.Id))
            {
                brokeredMessage.MessageId = message.Id;                
            }

            foreach (var attribute in message.Attributes)
            {
                brokeredMessage.Properties[attribute.Key] = attribute.Value;
            }
            return brokeredMessage;
        }

       
    }
}
