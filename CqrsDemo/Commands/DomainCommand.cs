using System;
using Pyxis.Cqrs.Commands;

namespace CqrsDemo.Commands
{
    public class DomainCommand : IDomainCommand
    {
        public DomainCommand(Guid id)
        {
            Id = id;
        }

        public string TrackingId { get; set; }
        public Guid Id { get; }
        public string Username { get; }
        public int? ExpectedVersion { get; }
    }
}