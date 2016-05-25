using System.Collections.Generic;

namespace Pyxis.Cqrs.Result
{
    public interface IDomainResultStoreQuery
    {
        IEnumerable<DomainResult> GetForTracking(string trackingId);
    }
}
