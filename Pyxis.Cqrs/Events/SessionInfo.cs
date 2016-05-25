using System;

namespace Pyxis.Cqrs.Events
{
    [Serializable]
    public class SessionInfo
    {
        public string Username { get; set; }
        public string TrackingId { get; set; }
    }
}