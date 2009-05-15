namespace Hineini.Utility {
    public class Helpers {
        public static bool StringHasValue(string candidate) {
            return !string.IsNullOrEmpty(candidate);
        }
    }
}