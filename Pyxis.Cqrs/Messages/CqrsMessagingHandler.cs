using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Infrastructure;
using Pyxis.Cqrs.Result;
using Pyxis.Messaging.Command;

namespace Pyxis.Cqrs.Messages
{
    public class CqrsMessagingHandler : IMessageDispatcher<ICommand>  
    {
        private readonly IEnumerable<IHandleCommand> _commandHandlers;
        private readonly IEnumerable<IHandleEvent> _eventHandlers;
        private IDictionary<string, HandlerStore> _handlersCache;
        private readonly IResultPublisher _resultPublisher;

        private readonly ILog _logger = LogManager.GetLogger(typeof (CqrsMessagingHandler));
        public CqrsMessagingHandler(IResultPublisher resultPublisher, IEnumerable<IHandleCommand> commandHandlers,
            IEnumerable<IHandleEvent> eventHandlers)
        {
            _resultPublisher = resultPublisher;
            _commandHandlers = commandHandlers;
            _eventHandlers = eventHandlers;
        }

        private void BuildHandlersCache()
        {
            _logger.Info("Building handler cache");
            _handlersCache = new Dictionary<string, HandlerStore>();
            var handlers = new List<IBaseHandler>(_commandHandlers);
            _logger.DebugFormat("Got {0} command handlers",_commandHandlers.Count());
            handlers.AddRange(_eventHandlers);
            _logger.DebugFormat("Got {0} event handlers", _eventHandlers.Count());
            foreach (var handler in handlers)
            {
                var handlerType = handler.GetType();
                var handledCommands = GetCqrsHandler(handlerType).ToArray();
                _logger.DebugFormat("Handler {0} will handle {1} commands", handlerType, handledCommands.Count());
                foreach (var commandHandler in handledCommands)
                {
                    var cqrsMessage = commandHandler.GetGenericArguments()[0];
                    if (!_handlersCache.ContainsKey(cqrsMessage.Name))
                    {
                        _handlersCache[cqrsMessage.Name] = new HandlerStore { CqrsMessageType = cqrsMessage, Handlers = new List<object>()};
                    }
                    _handlersCache[cqrsMessage.Name].Handlers.Add(handler);
                }
            }
        }

        private IEnumerable<Type> GetCqrsHandler(Type handlerType)
        {
            var handlers = handlerType.GetInterfaces().Where(i => i.IsGenericType && 
                i.GetGenericTypeDefinition().IsAssignableFrom(typeof(IBaseHandler<>)));
            return handlers;
        }

        public bool Dispatch(ICommand message)
        {
            _logger.InfoFormat("Dispatching message {0}", message.Type.Value);
            InitCache();
            HandlerStore handlerStore;
            _handlersCache.TryGetValue(message.Type.Value, out handlerStore);
            if (handlerStore != null)
            {
                var cqrsMessage = DomainMessageTranslator.TranslateCommand(message, handlerStore.CqrsMessageType);
                if (cqrsMessage is IDomainCommand)
                {
                    _logger.DebugFormat("Handling command");
                    HandleCommand(handlerStore, cqrsMessage as IDomainCommand);
                }
                else
                {
                    _logger.DebugFormat("Handling event");
                    HandleMessage(handlerStore, cqrsMessage);
                }
            }
            else
            {
                var cqrsMessage = DomainMessageTranslator.TranslateCommand(message,typeof(GenericDomainMessage));

                _logger.DebugFormat("No known handler for {0}, returning", message.Type.Value);
                var cqrsResult = new DomainResult
                {
                    Id = message.Id,
                    TrackingId = cqrsMessage.TrackingId,
                    ResultCode = ResultCode.OK
                };
                _resultPublisher.Publish(cqrsResult);
            }
            return true;
        }

        private void HandleCommand(HandlerStore handlerStore, IDomainCommand domainMessage)
        {
            var currentTuple = Tuple.Create(handlerStore, domainMessage);
            HandleMessage(currentTuple.Item1, currentTuple.Item2, 1);
       /*     do
            {
                lock (_runningCommands)
                {
                    if (_runningCommands.Contains(currentTuple.Item2.Id.ToString()))
                    {
                        _delayedCommands.Enqueue(currentTuple);
                        currentTuple = null;
                    }
                    else
                    {
                        _runningCommands.Add(domainMessage.Id.ToString());
                    }
                }

                if (currentTuple != null)
                {
                    HandleMessage(currentTuple.Item1, currentTuple.Item2,1);
                    lock (_runningCommands)
                    {
                        _runningCommands.Remove(currentTuple.Item2.Id.ToString());
                    }
                }
                _delayedCommands.TryDequeue(out currentTuple);
               
            }
            while (currentTuple != null);
    */    
    }

        private void HandleMessage(HandlerStore handlerStore, IDomainMessage domainMessage, int maxHandlers = int.MaxValue)
        {
            DomainResult domainResult;
            try
            {
                var targetHandlers = maxHandlers == int.MaxValue
                    ? handlerStore.Handlers
                    : handlerStore.Handlers.Take(maxHandlers);
                _logger.InfoFormat("Got {0} handlers to invoke", targetHandlers.Count());
                Parallel.ForEach(targetHandlers, handler => { handler.AsDynamic().Handle(domainMessage); });
                domainResult = new DomainResult(domainMessage, ResultCode.OK, string.Empty);
            }
            catch (Exception e)
            {
                _logger.Error("Error processing " + domainMessage.GetType(), e); 
                var errorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                _logger.Error("Error message " + errorMessage);
                domainResult = new DomainResult(domainMessage, ResultCode.Failed, errorMessage);
            }
            _resultPublisher.Publish(domainResult);
        }

        private void InitCache()
        {
            if (_handlersCache == null)
            {
                _logger.Debug("Creating handler cache");
                lock (this)
                {
                    // Avoid building twice
                    if (_handlersCache == null)
                    {
                        _logger.Debug("Creating handler cache in synched");
                        BuildHandlersCache();
                    }
                }
            }
        }
    }

    internal class GenericDomainMessage : IDomainMessage
    {
        public string TrackingId { get; set; }
        public Guid Id { get; set; }
    }
    internal class HandlerStore
    {
        internal Type CqrsMessageType { get; set; }
        internal IList<object> Handlers { get; set; }
    }

}
