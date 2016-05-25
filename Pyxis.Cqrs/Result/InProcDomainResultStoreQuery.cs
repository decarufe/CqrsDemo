using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using log4net;
using Pyxis.Cqrs.Bus;
using Pyxis.Persistance;

namespace Pyxis.Cqrs.Result
{
    public class InProcDomainResultStoreQuery : IDomainResultStoreQuery
    {
        private readonly IPersistanceStore _persistanceStore;
        private readonly ILog _logger = LogManager.GetLogger(typeof(InProcDomainResultStoreQuery));

        public InProcDomainResultStoreQuery(IPersistanceStore persistanceStore)
        {
            _persistanceStore = persistanceStore;
        }

        public IEnumerable<DomainResult> GetForTracking(string trackingId)
        {
            var retry = 5;
            var sleepTime = 50;
            DomainResult[] results = { new DomainResult { ResultCode = ResultCode.OK } };
            while (retry > 0)
            {
                try
                {
                    results = _persistanceStore.GetAll<DomainResult>().Where(x => x.TrackingId == trackingId).ToArray();
                    retry = 0;
                    break;
                }
                catch (Exception e)
                {
                    _logger.DebugFormat("Failed to read trackingId {0}, {1}", trackingId, e.Message);
                    if (--retry == 0)
                    {
                        _logger.Error(e.ToString());
                        return new DomainResult[] {new DomainResult {ResultCode = ResultCode.OK}};
                    }
                    else
                    {
                        Thread.Sleep(sleepTime);
                        sleepTime *= 2;
                    }
                }
            }

            return results;
        }
    }
}
