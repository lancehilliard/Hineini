using System;
using System.IO;
using System.Reflection;

namespace Hineini.Utility {
    public class Helpers {
        public static bool StringHasValue(string candidate) {
            return !String.IsNullOrEmpty(candidate);
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

        public static void WriteToExtraLog(string message, Exception exception) {
            WriteToFile(DateTime.Now + " > " + message, exception, GetWorkingDirectoryFileName("extra.log"), true);
        }

        public static string GetWorkingDirectoryFileName(string fileName) {
            return GetWorkingDirectory() + fileName;
        }

        private static string GetWorkingDirectory() {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\";
        }
    }
}