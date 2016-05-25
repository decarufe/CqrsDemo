using System.IO;
using Newtonsoft.Json;

namespace Pyxis.Persistance.Serialization
{
    public class JSonSerializer : ISerializer
    {
        public T Deserialize<T>(Stream stream) where T: class
        {
            stream.Position = 0;
            var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();
            var toReturn = JsonConvert.DeserializeObject<T>(content);
            return toReturn;
        }

        public Stream Serialize<T>(T content) where T : class
        {
            var toWrite = JsonConvert.SerializeObject(content);
            var stream = new MemoryStream();
            var sw = new StreamWriter(stream);
            sw.Write(toWrite);
            sw.Flush();
            return stream;
        }
    }
}
