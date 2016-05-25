using System.IO;

namespace Pyxis.Persistance.Filesystem
{
    public class FileUtils
    {
        public static void Write(string fileName, Stream content)
        {
            var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            using (stream)
            {
                content.Position = 0;
                content.CopyTo(stream);
                stream.Flush();
                stream.Close();
            }
        }

        public static Stream Read(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return stream;
        }
    }
}
