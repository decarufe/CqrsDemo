using System;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Snapshots;
using Pyxis.Cqrs.Tests.Domain.Events;

namespace Pyxis.Cqrs.Tests.Domain.Objects
{
    public class TestAggregate : SnapshotAggregateRoot<TestAggregate.State>
    {
        public class State : Snapshot
        {
            public string Name { get; set; }
        }

        private State _state = new State();

        public TestAggregate()
        {
        }

        public TestAggregate(Guid id)
        {
            Id = id;
            ApplyChange(new TestAggregateCreated(id));
        }

        public void TestMethod()
        {
            ApplyChange(new TestMethodCalled(Id));
        }

        public void TestInteractWithOtherAggregate(Guid id, ISession session)
        {
            var otherAggregate = new TestOtherAggregate(id);
            session.Add(otherAggregate);
        }

        public int CreateEntity<TEntity>() where TEntity : Entity
        {
            var testEntity = new TestEntity(this);
            AddEntity(testEntity);
            return 0;
        }

        protected override State CreateSnapshot
        {
            get { return _state; }
        }

        protected override void RestoreFromSnapshot(State snapshot)
        {
            _state = snapshot;
        }
    }
}