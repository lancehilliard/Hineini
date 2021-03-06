﻿using System;
using System.IO;
using System.Reflection;

namespace Hineini.Utility {
    public class Helpers {
        static readonly TextWriter textWriter = TextWriter.Synchronized(File.CreateText(GetWorkingDirectoryFileName("extra.log")));
        private static bool extraLogEnabled;

        public static bool StringHasValue(string candidate) {
            return !String.IsNullOrEmpty(candidate);
        }

        public static void Dispose() {
            textWriter.Dispose();
        }

        public static bool ExtraLogEnabled {
            set { extraLogEnabled = value; }
        }

        private static void WriteToFile(string errorDescriptor, Exception exception) {
            textWriter.WriteLine(errorDescriptor);
            if (exception != null) {
                textWriter.Write(exception);
            }
            textWriter.Flush();
        }

        public static void WriteToExtraLog(string message, Exception exception) {
            if (extraLogEnabled) {
                WriteToFile(Constants.CURRENT_VERSION + " | " + DateTime.Now + " > " + message, exception);
            }
        }

        public static string GetWorkingDirectoryFileName(string fileName) {
            return GetWorkingDirectory() + fileName;
        }

        private static string GetWorkingDirectory() {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\";
        }
    }
}