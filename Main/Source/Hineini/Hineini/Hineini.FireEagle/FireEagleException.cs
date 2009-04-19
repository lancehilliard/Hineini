using System;

namespace Hineini.FireEagle {
    [Serializable]
    public class FireEagleException : Exception
    {
        internal FireEagleException()
        {
        }

        internal FireEagleException(string message)
            : base(message)
        {
        }

        internal FireEagleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}