using System;
using System.Collections.Generic;

namespace Pyxis.Cqrs.Events
{
    public interface IDomainEventStore
    {
        void Save(IEnumerable<DomainEvent> events, SessionInfo sessionInfo);
        IEnumerable<DomainEvent> Get(Guid aggregateId, int fromVersion);
    }
}