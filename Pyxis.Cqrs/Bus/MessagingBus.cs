using System.Linq;
using log4net;
using Newtonsoft.Json;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Result;
using Pyxis.Messaging;
using Pyxis.Messaging.Command;

namespace Pyxis.Cqrs.Bus
{
    public class MessagingBus : ICommandSender, IDomainEventPublisher
    {
        private readonly ICommandQueue _commandQueue;
        private readonly IResultPublisher _resultPublisher;
        private readonly IResultAwaiter _resultAwaiter;
        private readonly ILog _logger = LogManager.GetLogger(typeof (MessagingBus));
        public MessagingBus(ICommandQueue commandQueue, IResultPublisher resultPublisher, IResultAwaiter resultAwaiter)
        {
            _commandQueue = commandQueue;
            _resultAwaiter = resultAwaiter;
            _resultPublisher = resultPublisher;
        }
        public DomainResult SendAndWait<T>(T command) where T : IDomainCommand
        {
            _logger.DebugFormat("Send and wait for {0}", command.GetType());
            var trackedCommand = SendCommand(command);
            return _resultAwaiter.WaitForResults(trackedCommand);
        }
        public DomainResult Send<T>(T command) where T : IDomainCommand
        {
            _logger.DebugFormat("Send {0}", command.GetType());
            var trackedCommand = SendCommand(command);
            return _resultAwaiter.WaitForCommand(trackedCommand);
        }

        private TrackedCommand SendCommand<T>(T command) where T : IDomainCommand
        {
            var messagingCommand = DomainMessageTranslator.TranslateCommand(command, new CommandType(command.GetType().Name));
            _resultPublisher.Publish(new DomainResult(command, ResultCode.Unknown));
            _logger.InfoFormat("Queuing {0}", command.GetType());
            _commandQueue.QueueCommand(messagingCommand);
            return messagingCommand;
        }
        public void Publish<T>(T[] events, string trackingId = null) where T : DomainEvent
        {
            if (!events.Any()) return;;
            _logger.InfoFormat("Publishing {0} events", events.Count());
            var commands = new ICommand[events.Length];
            for (var eventCount = 0; eventCount < events.Length;eventCount++)
            {
                var messagingCommand = DomainMessageTranslator.TranslateCommand(events[eventCount],
                    new CommandType(events[eventCount].GetType().Name));
                _logger.DebugFormat("Translated {0} ", events[eventCount].GetType().Name);
                messagingCommand.TrackingId = trackingId;
                var json = JsonConvert.SerializeObject(events[eventCount], Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All});
                _logger.InfoFormat("{0} Id:{1}, Tracking: {2}", events[eventCount].GetType().Name, events[eventCount].Id, trackingId);
                _logger.Debug(json);
                _resultPublisher.Publish(new DomainResult(events[eventCount], ResultCode.Unknown));
                commands[eventCount] = messagingCommand;
            }
            _commandQueue.QueueCommand(commands);
        }
    }
}



