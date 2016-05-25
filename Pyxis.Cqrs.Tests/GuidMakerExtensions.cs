using System;

namespace Pyxis.Cqrs.Tests
{
    public static class GuidMakerExtensions
    {
        public static Guid MakeGuid(this int id)
        {
            return new Guid(id, 0, 0, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        }
    }
}