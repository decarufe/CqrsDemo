using System;
using System.Collections.Generic;

namespace Pyxis.Cqrs.Events
{
    public interface IDomainEventQuery
    {
        IEnumerable<DomainEvent> Get(Guid aggregateId, int fromVersion);
    }
}
