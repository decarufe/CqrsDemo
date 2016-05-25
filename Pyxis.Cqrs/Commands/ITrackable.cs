using System;
using Pyxis.Core.Id;

namespace Pyxis.Cqrs.Commands
{
    public interface ITrackable 
    {
        string TrackingId { get; set; }
    }
}
