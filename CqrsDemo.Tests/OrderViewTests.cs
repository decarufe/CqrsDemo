using System.Collections.Generic;
using System.Linq;
using CqrsDemo.Domain;
using CqrsDemo.EventHandlers;
using CqrsDemo.Events;
using CqrsDemo.Services;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Pyxis.Cqrs.Tests;

namespace CqrsDemo.Tests
{
    public class OrderViewTests
    {
        [TestFixture()]
        public class OrderListTest
        {
            [Test]
            public void Test()
            {
                var handler = new OrderEventHandler();
                handler.Handle(new OrderCreated(1.MakeGuid()));
                handler.Handle(new OrderLineAdded(1.MakeGuid(), new OrderLine()
                {
                    Quantity = 3,
                    ProductName = "MyProduct"
                }));

                var view = new OrderView();
                view.GetAll().Single().ShouldBeEquivalentTo(new OrderDto()
                {
                    Id = 1.MakeGuid(),
                    OrderLines = new []
                    {
                        new OrderLineDto()
                        {
                            Quantity = 3,
                            ProductName = "MyProduct"
                        }
                    }
                });
            }
        }
    }
}