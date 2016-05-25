using System;
using Pyxis.Cqrs.Events;

namespace Pyxis.Cqrs.Domain
{
    public interface IDomainRepository
    {
        void Save<T>(T aggregate, SessionInfo sessionInfo, int? expectedVersion = null) where T : AggregateRoot;
        T Get<T>(Guid aggregateId) where T : AggregateRoot;
    }
}