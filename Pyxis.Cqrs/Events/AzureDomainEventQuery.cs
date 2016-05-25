using System;
using System.Collections.Generic;
using System.Linq;
using Pyxis.Persistance.Query;

namespace Pyxis.Cqrs.Events
{
    public class AzureDomainEventQuery : IDomainEventQuery
    {
        private readonly IPersistanceQuery _queryStore;

        public AzureDomainEventQuery(IPersistanceQuery queryStore)
        {
            _queryStore = queryStore;
        }

        public IEnumerable<DomainEvent> Get(Guid aggregateId, int fromVersion)
        {
            var aggregates = _queryStore.Query<PersistedDomainEvent>(new []
            {
                QueryElement.FieldEquals("EventId", aggregateId.ToString()),
                QueryElement.FieldCompare("Version", FieldComparison.GreaterEquals, fromVersion),
            });

            return aggregates.OrderBy(x => x.Version).Select(x => x.Source).ToList();
        }
    }
}
