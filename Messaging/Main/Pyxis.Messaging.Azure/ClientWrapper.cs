using System;
using System.Threading;
using Microsoft.ServiceBus.Messaging;

namespace Pyxis.Messaging.Azure
{
    public abstract class ClientWrapper<T> : IDisposable where T: ClientEntity
    {
        public T Client { get;  private set; }

        protected void Initialize(INodeIdentifier nodeIdentifier, IChannelDescriptor eventChannelDescriptor)
        {
            try
            {
                Client = CreateClient(nodeIdentifier, eventChannelDescriptor);
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                Client = CreateClient(nodeIdentifier, eventChannelDescriptor);
            }
        }

        public abstract T CreateClient(INodeIdentifier nodeIdentifier, IChannelDescriptor eventChannelDescriptor);

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            Client.Close();
        }
    }
}