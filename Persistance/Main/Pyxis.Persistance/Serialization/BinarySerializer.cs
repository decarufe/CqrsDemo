using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pyxis.Persistance.Serialization
{
    public class BinarySerializer : ISerializer
    {
        public T Deserialize<T>(Stream stream) where T : class
        {
            stream.Position = 0;
            var formatter = new BinaryFormatter();
            var toReturn = (T)formatter.Deserialize(stream);
            return toReturn;
        }

        public Stream Serialize<T>(T content) where T : class
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, content);
            return stream;
        }
    }
}
