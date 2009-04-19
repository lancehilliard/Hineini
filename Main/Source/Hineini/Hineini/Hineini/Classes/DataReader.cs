using System.IO;
using System.Reflection;

namespace Hineini {
    public static class DataReader {
        public const string TOKEN_FILENAME = "first";
        public const string SECRET_FILENAME = "second";

        private static string GetFileName(string which) {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\" + which + ".dat";
        }

        public static byte[] ReadData(string which) {
            string fileName = GetFileName(which);
            if (File.Exists(fileName)) {
                using (FileStream stream = new FileStream(fileName, FileMode.Open)) {
                    BinaryReader reader = new BinaryReader(stream);
                    byte[] bytes = reader.ReadBytes((int)stream.Length);
                    return bytes;
                }
            }
            else return null;
        }

        public static void WriteData(byte[] data, string which) {
            using (FileStream stream = new FileStream(GetFileName(which), FileMode.Create)) {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(data);
            }
        }

    }
}
