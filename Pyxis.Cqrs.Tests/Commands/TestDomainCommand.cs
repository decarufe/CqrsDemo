using System;
using Pyxis.Cqrs.Commands;

namespace Pyxis.Cqrs.Tests.Commands
{
    public class TestDomainCommand : IDomainCommand
    {
        public Guid Id { get; set; }
        public string Username { get; private set; }
        public int? ExpectedVersion { get; set; }
        public string TrackingId { get; set; }
    }
}
