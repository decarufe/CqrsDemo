using System;

namespace Pyxis.Cqrs.Commands
{
    [Serializable]
    public abstract class DomainCommand : IDomainCommand
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public int? ExpectedVersion { get; private set; }
        public string TrackingId { get; set;  }
        protected DomainCommand(Guid id, string username, int? expectedVersion)
        {
            Id = id;
            Username = username;
            ExpectedVersion = expectedVersion;
        }
    }
}