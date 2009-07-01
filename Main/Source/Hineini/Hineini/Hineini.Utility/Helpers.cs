using System;
using System.IO;

namespace Hineini.Utility {
    public class Helpers {
        public static bool StringHasValue(string candidate) {
            return !string.IsNullOrEmpty(candidate);
        }

        public static void WriteToFile(string errorDescriptor, Exception exception, string path, bool append) {
            StreamWriter streamWriter = new StreamWriter(path, append);
            streamWriter.WriteLine(errorDescriptor);
            if (exception != null) {
                streamWriter.Write(exception);
            }
            streamWriter.Flush();
            streamWriter.Close();
        }
    }
}