using System.Collections.Generic;
using System.Linq;
using Pyxis.Persistance;
using Pyxis.Persistance.Query;

namespace Pyxis.Cqrs.Result
{
    public class AzureDomainResultStoreQuery : IDomainResultStoreQuery
    {
        private readonly IPersistanceQuery _persistanceQuery;

        public AzureDomainResultStoreQuery(IPersistanceQuery persistanceQuery)
        {
            _persistanceQuery = persistanceQuery;
        }

        public IEnumerable<DomainResult> GetForTracking(string trackingId)
        {
            var toReturn = _persistanceQuery.Query<DomainResult>(QueryElement.FieldEquals("TrackingId", trackingId));
            return toReturn;
        }
    }
}
