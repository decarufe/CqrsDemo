using System;
using Pyxis.Cqrs.Commands;

namespace Pyxis.Cqrs.Messages
{
    public interface IDomainMessage : ITrackable
    {
        Guid Id { get; }
    }
}