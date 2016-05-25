using System;

namespace Pyxis.Persistance.Serialization
{
    public class SerializerFactory
    {
        public static bool PreferJsonSerialization { get; set; }
        public static ISerializer GetSerializer<T>(T content)
        {
            return GetSerializer(typeof (T));
        }

        public static ISerializer GetSerializer(Type contentType) 
        {
            if (!PreferJsonSerialization && contentType.IsSerializable)
            {
                return new BinarySerializer();
            }
            return new JSonSerializer();
        }
    }
}
