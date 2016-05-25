using System;
using Pyxis.Messaging.Command;

namespace Pyxis.Cqrs.Commands
{
    public class TrackedCommand : Command, ITrackable
    {
        private const string TrackingIdKey = "_trackingId";

        public TrackedCommand(ICommand source) : base(source)
        {
            Attributes[TrackingIdKey] = Guid.NewGuid().ToString();
        }

        public TrackedCommand(string trackingId,CommandType type, string content = null, bool async = true) : base(type, content, async)
        {
            Attributes[TrackingIdKey] = trackingId;
        }

        public string TrackingId
        {
            get { return Attributes[TrackingIdKey] as string; }
            set { Attributes[TrackingIdKey] = value; }
        }
    }
}
