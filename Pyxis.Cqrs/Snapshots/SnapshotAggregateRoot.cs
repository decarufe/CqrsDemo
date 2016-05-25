using Pyxis.Cqrs.Domain;

namespace Pyxis.Cqrs.Snapshots
{
    public abstract class SnapshotAggregateRoot<T> : AggregateRoot where T : Snapshot
    {
        public T GetSnapshot()
        {
            var snapshot = CreateSnapshot;
            snapshot.Id = Id;
            return snapshot;
        }

        public void Restore(T snapshot)
        {
            Id = snapshot.Id;
            Version = snapshot.Version;
            RestoreFromSnapshot(snapshot);
        }

        protected abstract T CreateSnapshot { get; }

        protected abstract void RestoreFromSnapshot(T snapshot);
    }
}