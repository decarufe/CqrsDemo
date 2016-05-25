using System;
using System.Collections.Generic;
using Microsoft.ServiceBus.Messaging;
using NUnit.Framework;

namespace Pyxis.Messaging.Azure.Tests
{
    [TestFixture]
    public class ClientWrapperTests
    {
        [Test]
        public void TestWrapperRetriesToCreateClientUponException()
        {
            var wrapper = new TestWrapper();
            try
            {
                wrapper.Initialize();
            }
            catch (Exception)
            {
                // Expected, fails in the end                
            }
            Assert.AreEqual(2, wrapper.Invocations.Count);
            var elapsed = (wrapper.Invocations[1] - wrapper.Invocations[0]);
            Assert.AreEqual(1, (int)elapsed.TotalSeconds);
        }
    }

    class TestWrapper : ClientWrapper<TopicClient>
    {
        public IList<DateTime> Invocations = new List<DateTime>();
        public override TopicClient CreateClient(INodeIdentifier nodeIdentifier, IChannelDescriptor eventChannelDescriptor)
        {
            Invocations.Add(DateTime.Now);
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            base.Initialize(null,null);
        }
    }
}
