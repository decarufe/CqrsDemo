using System;
using Pyxis.Cqrs.Domain;
using Pyxis.Cqrs.Tests.Domain.Events;

namespace Pyxis.Cqrs.Tests.Domain.Objects
{
    public class TestOtherAggregate : AggregateRoot
    {
        public TestOtherAggregate()
        {
        }

        public TestOtherAggregate(Guid id)
        {
            Id = id;
            ApplyChange(new TestOtherAggregateCreated(Id));
        }

        public void TestOtherMethod()
        {
            ApplyChange(new TestOtherMethodCalled(Id));
        }
    }
}