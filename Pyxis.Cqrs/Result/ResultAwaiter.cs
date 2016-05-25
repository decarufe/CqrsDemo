using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using Pyxis.Cqrs.Commands;

namespace Pyxis.Cqrs.Result
{
    public class ResultAwaiter : IResultAwaiter
    {
        private readonly IDomainResultStore _resultStore;
        private int _sleepInterval = 100;
        private readonly long _waitTimeout;
        private readonly ILog _logger = LogManager.GetLogger(typeof (ResultAwaiter));
        public ResultAwaiter(IDomainResultStore resultStore, ITimeoutProvider timeoutProvider)
        {
            _resultStore = resultStore;
            _waitTimeout = timeoutProvider.Timeout;
            _logger.DebugFormat("Timeout is {0}", _waitTimeout);
        }

        public DomainResult WaitForCommand(ITrackable command)
        {
            return WaitForCompletion(command, true);
        }

        public DomainResult WaitForResults(ITrackable command)
        {
            return WaitForCompletion(command, false);
        }

        private DomainResult WaitForCompletion(ITrackable command, bool commandOnly)
        {
            _logger.DebugFormat("Wait from completion, ID is {0}, command only is {1}", command.TrackingId, commandOnly);
            IEnumerable<DomainResult> results;
            bool hasOngoingProcessing;
            var start = DateTime.Now;
            do
            {
                results = _resultStore.Get(command.TrackingId).ToArray();
                if (commandOnly)
                {
                    results = results.Where(x => x.IsCommand).ToArray();
                }
                hasOngoingProcessing = results.Any(x => x.ResultCode == ResultCode.Unknown);
                _logger.DebugFormat("Has ongoing for ID {0} : {1}", command.TrackingId, hasOngoingProcessing);
                if (hasOngoingProcessing)
                {
                    Thread.Sleep(_sleepInterval);
                }
            }
            while (hasOngoingProcessing && (DateTime.Now - start).TotalMilliseconds < _waitTimeout);
            _logger.DebugFormat("Done waiting for ID {0}, has ongoing: {1}, has results: {2}", command.TrackingId, hasOngoingProcessing, results.Any());
            _logger.DebugFormat("Last known results for ID {0} are", command.TrackingId);
            foreach (var cqrsResult in results)
            {
                _logger.DebugFormat("Result: {0}, isCommand: {1}", cqrsResult.ResultCode, cqrsResult.IsCommand);
            }

            if (hasOngoingProcessing || !results.Any())
            {
                return new DomainResult {TrackingId = command.TrackingId, ResultCode = ResultCode.Unknown, };
            }
            _resultStore.Delete(command.TrackingId);
           
            return results.OrderByDescending(x=> x.ResultCode).FirstOrDefault();
        }
    }
}
