using System.Collections.Generic;
using System.Linq;
using log4net;
using Pyxis.Persistance;

namespace Pyxis.Cqrs.Result
{
    public class DomainResultPersistanceStore : IDomainResultStore
    {
        private readonly IPersistanceStore _persistanceStore;
        private readonly IDomainResultStoreQuery _domainResultStoreQuery;
        private readonly ILog _logger = LogManager.GetLogger(typeof (DomainResultPersistanceStore));
        public DomainResultPersistanceStore(IPersistanceStore persistanceStore, IDomainResultStoreQuery domainResultStoreQuery)
        {
            _persistanceStore = persistanceStore;
            _domainResultStoreQuery = domainResultStoreQuery;
        }

        public IEnumerable<DomainResult> Get(string trackingId)
        {
            IEnumerable<DomainResult> toReturn = new DomainResult[0];
            if (!string.IsNullOrEmpty(trackingId))
            {
                toReturn = _domainResultStoreQuery.GetForTracking(trackingId);
            }
            _logger.DebugFormat("Getting for tracking {0}, found {1} results", trackingId, toReturn.Count());
            return toReturn;
        }

        public void Save(DomainResult result)
        {
            _logger.DebugFormat("Saving result {0} for tracking {1}", result.ResultCode, result.TrackingId);
            _persistanceStore.Save(result);
        }

        public void Delete(string trackingId)
        {
            _logger.DebugFormat("Delete for tracking {0}", trackingId);
            var toDelete = Get(trackingId).ToArray();
            if (toDelete.Any())
            {
                _logger.DebugFormat("Found {0} results", toDelete.Count());
                _persistanceStore.Delete<DomainResult>(toDelete.Select(x => x.Id).ToArray());
            }
            else
            {
                _logger.DebugFormat("Found no results");
            }
        }
    }
}
