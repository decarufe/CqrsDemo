using System.Linq;
using Pyxis.Cqrs.Domain;

namespace Pyxis.Cqrs.Snapshots
{
    public class NoSnapshotStrategy : SnapshotStrategyBase
    {
        public override bool ShouldMakeSnapShot(AggregateRoot aggregate)
        {
            return false;
        }
    }
}