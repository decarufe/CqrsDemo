using System;
using Pyxis.Cqrs.Domain;

namespace Pyxis.Cqrs.Snapshots
{
    public interface ISnapshotStrategy
    {
        bool ShouldMakeSnapShot(AggregateRoot aggregate);
        bool IsSnapshotable(Type aggregateType);
    }
}