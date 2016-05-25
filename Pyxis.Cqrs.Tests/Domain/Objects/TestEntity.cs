using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Tests.Domain.Events;

namespace Pyxis.Cqrs.Tests.Domain.Objects
{
    public class TestEntity : Entity
    {
        public TestEntity(AggregateRoot aggregateRoot)
        {
            AggregateRoot = aggregateRoot;
            ApplyChange(new TestEntityCreated(aggregateRoot.Id));
        }
    }
}