using System.Configuration;

namespace Pyxis.Cqrs.Result
{
    public class TimeoutProvider : ITimeoutProvider
    {
        private const long DefaultTimeout = 5000;

        public long Timeout { get; set; }

        public TimeoutProvider(long timeOut)
        {
            Timeout = (timeOut == 0) ?  DefaultTimeout : timeOut;
        }
    }
}
