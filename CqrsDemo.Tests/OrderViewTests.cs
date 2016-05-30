using System.Linq;
using CqrsDemo.Domain;
using CqrsDemo.EventHandlers;
using CqrsDemo.Events;
using CqrsDemo.Services;
using FluentAssertions;
using NUnit.Framework;
using Pyxis.Cqrs.Tests;

namespace CqrsDemo.Tests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class OrderViewTests
    {
        [TestFixture()]
        public class OrderListTest
        {
            [Test]
            public void Test()
            {
                OrderView orderView = new OrderView();
                var handler = new OrderEventHandler(orderView);
                handler.Handle(new OrderCreated(1.MakeGuid()));
                handler.Handle(new OrderLineAdded(1.MakeGuid(), new OrderLine(3, "MyProduct")));

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