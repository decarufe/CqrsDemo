using System;
using Pyxis.Core.Id;

namespace Pyxis.Persistance.Tests.Query
{
    public class QueryableTestObject : IIdentifiable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? TimeStamp { get; set; }
        public int? IntVersion { get; set; }
        public long? LongVersion { get; set; }
        public double? DoubleVersion { get; set; }
        public string StringVersion { get; set; }
    }
}
