using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Snapshots;
using Pyxis.Cqrs.Tests.Domain.Events;
using Pyxis.Cqrs.Tests.Domain.Objects;

namespace Pyxis.Cqrs.Tests.Domain
{
    [TestFixture]
    public class AggregateSnapshotTest : AggregateSpecification<TestAggregate>
    {
        private readonly Mock<ISnapshotStrategy> _snapshotStragegyMock = new Mock<ISnapshotStrategy>();
        private Mock<ISnapshotStore> _snapshotStore;

        protected override IEnumerable<DomainEvent> Given()
        {
            return new List<DomainEvent>();
        }

        protected override TestAggregate When(ISession session, IDomainRepository domainRepository)
        {
            var aggregate = new TestAggregate(GetGuid(1));
            return aggregate;
        }

        protected override ISnapshotStrategy GetSnapshotStrategy()
        {
            _snapshotStragegyMock.Setup(m => m.IsSnapshotable(It.IsAny<Type>())).Returns(true);
            _snapshotStragegyMock.Setup(m => m.ShouldMakeSnapShot(It.IsAny<TestAggregate>())).Returns(true);
            return _snapshotStragegyMock.Object;
        }

        protected override ISnapshotStore GetSnapshotstorage()
        {
            _snapshotStore = new Mock<ISnapshotStore>();
            return _snapshotStore.Object;
        }

        [Then]
        public void Snapshot_should_be_saved()
        {
            _snapshotStore.Verify(m => m.Save(It.IsAny<TestAggregate.State>()));
        }
    }
}