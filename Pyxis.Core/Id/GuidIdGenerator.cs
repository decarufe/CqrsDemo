using System;

namespace Pyxis.Core.Id
{
    public class GuidIdGenerator : IIdGenerator
    {
        public string GenerateId(string seed = "")
        {
            return Guid.NewGuid().ToString();
        }
    }
}
