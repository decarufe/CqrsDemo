using System.Collections.Generic;
using CqrsDemo.CommandHandlers;
using CqrsDemo.Commands;
using CqrsDemo.Domain;
using CqrsDemo.Events;
using NUnit.Framework;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Tests;
using Pyxis.Cqrs.Tests.TestHelpers;

namespace CqrsDemo.Tests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OrderDomainTests
    {
        [TestFixture]
        public class OrderCreation : DynamicSpecification
        {
            protected override dynamic BuildHandler()
            {
                return new OrderCommandHandler(Session);
            }

            protected override IEnumerable<DomainEvent> Given()
            {
                return new DomainEvent[] {};
            }

            protected override dynamic When()
            {
                return new CreateOrder(1.MakeGuid());
            }

            [Then]
            public void Then()
            {
                ExpectEvents(new OrderCreated(1.MakeGuid())
                {
                    Version = 1
                });
            }
        }

        [TestFixture]
        public class OrderAddLine : DynamicSpecification
        {
            protected override dynamic BuildHandler()
            {
                return new OrderCommandHandler(Session);
            }

            protected override IEnumerable<DomainEvent> Given()
            {
                return new DomainEvent[]
                {
                    new OrderCreated(1.MakeGuid()) {Version = 1}
                };
            }

            protected override dynamic When()
            {
                return new AddOrderLine(1.MakeGuid(), new OrderLine(3, "MyProduct"));
            }

            [Then]
            public void Then()
            {
                ExpectEvents(new OrderLineAdded(
                    1.MakeGuid(),
                    new OrderLine(3, "MyProduct"))
                {
                    Version = 2
                });
            }
        }

        [TestFixture]
        public class OrderComplete : DynamicSpecification
        {
            protected override dynamic BuildHandler()
            {
                return new OrderCommandHandler(Session);
            }

            protected override IEnumerable<DomainEvent> Given()
            {
                return new DomainEvent[]
                {
                    new OrderCreated(1.MakeGuid()) {Version = 1}
                };
            }

            protected override dynamic When()
            {
                return new CompleteOrder(1.MakeGuid());
            }

            [Then]
            public void Then()
            {
                ExpectEvents(new OrderCompleted(1.MakeGuid())
                {
                    Version = 2
                });
            }
        }
    }
}