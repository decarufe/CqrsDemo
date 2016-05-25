using System;

namespace Pyxis.Persistance.Container
{
    [Serializable]
    public class PersistedString : PersistedObject
    {
        public string GetContent()
        {
            return GetContent<string>();
        }
    }
}