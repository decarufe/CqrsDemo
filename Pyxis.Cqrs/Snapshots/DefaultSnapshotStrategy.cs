using System.Linq;
using Pyxis.Cqrs.Domain;

namespace Pyxis.Cqrs.Snapshots
{
    public class DefaultSnapshotStrategy : SnapshotStrategyBase
    {
        private const int SnapshotInterval = 15;

        public override bool ShouldMakeSnapShot(AggregateRoot aggregate)
        {
            var i = aggregate.Version;

            for (var j = 0; j < aggregate.GetUncommittedChanges().Count(); j++)
                if (++i%SnapshotInterval == 0 && i != 0) return true;
            return false;
        }
    }
}