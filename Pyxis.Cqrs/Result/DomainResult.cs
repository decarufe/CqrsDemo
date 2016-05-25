using System;
using Pyxis.Core.Id;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Messages;

namespace Pyxis.Cqrs.Result
{
    public class DomainResult : IIdentifiable, ITrackable
    {
        public DomainResult()
        {
            
        }
        public DomainResult(IDomainMessage trackedMessage, ResultCode resultCode, string resultMessage = "")
        {
            Id = trackedMessage.Id.ToString();
            TrackingId = trackedMessage.TrackingId;
            ResultCode = resultCode;
            ResultMessage = resultMessage;
            IsCommand = trackedMessage is IDomainCommand;
            CreationTs = DateTime.Now;
        }
     
        public string Id { get; set; }
        public string TrackingId { get; set; }
        public bool IsCommand { get; set; }
        public ResultCode ResultCode { get;  set; }
        public string ResultMessage{ get;  set; }
        public bool IsSuccess { get { return ResultCode == ResultCode.OK; } }
        public DateTime CreationTs { get; set; }
    }

    public enum ResultCode
    {
        // Went well
        OK,
        // Never got the result
        Unknown,
        // Did not work
        Failed
    }
}
