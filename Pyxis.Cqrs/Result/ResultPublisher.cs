using System;
using System.Threading.Tasks;

namespace Pyxis.Cqrs.Result
{
    public class ResultPublisher : IResultPublisher
    {
        private readonly IDomainResultStore _resultStore;
        private readonly int _timeout;

        public ResultPublisher(IDomainResultStore resultStore, ITimeoutProvider timeoutProvider)
        {
            _resultStore = resultStore;
            // Allow twice the read time
            _timeout = (int) (timeoutProvider.Timeout * 2);
        }

        public void Publish(DomainResult result)
        {
            _resultStore.Save(result);
            
            Execute(() => _resultStore.Delete(result.TrackingId), _timeout);
        }
        private async void Execute(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            action();
        }
    }
}
