using System.Collections.Generic;

namespace Pyxis.Cqrs.Result
{
    public interface IDomainResultStore
    {
        IEnumerable<DomainResult> Get(string trackingId);
        void Save(DomainResult result);
        void Delete(string trackingId);
    }
}
