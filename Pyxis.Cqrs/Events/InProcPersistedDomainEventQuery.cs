using System;
using System.Collections.Generic;
using System.Linq;
using Pyxis.Persistance;

namespace Pyxis.Cqrs.Events
{
    public class InProcPersistedDomainEventQuery : IDomainEventQuery
    {
        private readonly IPersistanceStore _persistanceStore;

        public InProcPersistedDomainEventQuery(IPersistanceStore persistanceStore)
        {
            _persistanceStore = persistanceStore;
        }

        public IEnumerable<DomainEvent> Get(Guid aggregateId, int fromVersion)
        {
            var aggregates = _persistanceStore.GetAll<PersistedDomainEvent>().Where(x => x.EventId == aggregateId.ToString() && x.Version >= fromVersion);
            return aggregates.OrderBy(x=> x.Version).Select(x=> x.Source).ToList();
        }
    }
}
