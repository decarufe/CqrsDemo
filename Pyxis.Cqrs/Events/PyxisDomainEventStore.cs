using System;
using System.Collections.Generic;
using log4net;
using Pyxis.Persistance;

namespace Pyxis.Cqrs.Events
{
    public class PyxisDomainEventStore : IDomainEventStore
    {
        private readonly IPersistanceStore _persistanceStore;
        private readonly IDomainEventQuery _domainEventQuery;
        private readonly ILog _logger = LogManager.GetLogger(typeof (PyxisDomainEventStore));
        public PyxisDomainEventStore(IPersistanceStore persistanceStore, IDomainEventQuery domainEventQuery)
        {
            _persistanceStore = persistanceStore;
            _domainEventQuery = domainEventQuery;
        }

        public void Save(IEnumerable<DomainEvent> events, SessionInfo sessionInfo)
        {
            foreach (var @event in events)
            {
                @event.SessionInfo = sessionInfo;
                var toSave = new PersistedDomainEvent(@event);
                var shouldBeNull =_persistanceStore.Get<PersistedDomainEvent>(toSave.Id);
                if (shouldBeNull != null)
                {
                    var error = string.Format("Cannot save an event with an existing ID! Id: {0}, type={1}", toSave.Id,
                        @event.GetType());
                    _logger.Error(error);
                    throw new ArgumentException(error);
                }
                _persistanceStore.Save(toSave);
            }
        }
        public IEnumerable<DomainEvent> Get(Guid aggregateId, int fromVersion)
        {
            return _domainEventQuery.Get(aggregateId, fromVersion);
        }
    }
    
}

