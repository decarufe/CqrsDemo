using System;
using Pyxis.Core.Id;

namespace Pyxis.Persistance.Container
{
    [Serializable]
    public class PersistedObject : IIdentifiable
    {
        public object Content { get; set; }
        public string Id { get; set; }

        public T GetContent<T>() where T : class
        {
            return Content as T;
        }
    }
}