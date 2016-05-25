using System.IO;

namespace Pyxis.Persistance.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(Stream sourceStream) where T : class;
        Stream Serialize<T>(T content) where T : class;
    }
}
