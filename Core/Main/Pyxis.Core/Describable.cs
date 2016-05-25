using Pyxis.Core.Id;

namespace Pyxis.Core
{
    public class Describable : IIdentifiable
    {
        public string Id { get; set; }
        public string Description { get; set; }
    }
}
