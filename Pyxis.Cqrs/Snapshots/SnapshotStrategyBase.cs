using System;
using Pyxis.Cqrs.Domain;

namespace Pyxis.Cqrs.Snapshots
{
    public abstract class SnapshotStrategyBase : ISnapshotStrategy
    {
        public virtual bool IsSnapshotable(Type aggregateType)
        {
            if (aggregateType.BaseType == null)
                return false;
            if (aggregateType.BaseType.IsGenericType &&
                aggregateType.BaseType.GetGenericTypeDefinition() == typeof (SnapshotAggregateRoot<>))
                return true;
            return IsSnapshotable(aggregateType.BaseType);
        }

        public abstract bool ShouldMakeSnapShot(AggregateRoot aggregate);
    }
}