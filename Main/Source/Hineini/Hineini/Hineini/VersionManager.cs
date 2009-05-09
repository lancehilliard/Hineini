using System;
using System.IO;
using System.Net;
using Hineini.Utility;

namespace Hineini {
    public class VersionManager {
        public static bool KnownRecommendedVersionDiffersFromCurrentVersion() {
            bool result = false;
            string recommendedVersion = GetRecommendedVersion();
            if (recommendedVersion.Length > 0) {
                bool recommendedVersionDiffersFromCurrentVersion = !Constants.CURRENT_VERSION.Equals(recommendedVersion);
                result = recommendedVersionDiffersFromCurrentVersion;
            }
            return result;
        }

        private static string GetRecommendedVersion() {
            string result = string.Empty;
            WebRequest webRequest = WebRequest.Create(Constants.RECOMMENDED_VERSION_URL);
            HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string html = streamReader.ReadToEnd();
            int recommendedVersionDelimiterStartIndex = html.IndexOf(Constants.RECOMMENDED_VERSION_HTML_DELIMITER_START);
            if (recommendedVersionDelimiterStartIndex >= 0) {
                string recommendedVersion = html.Substring(recommendedVersionDelimiterStartIndex);
                int recommendedVersionDelimiterEndIndex = recommendedVersion.IndexOf(Constants.RECOMMENDED_VERSION_HTML_DELIMITER_END);
                recommendedVersion = recommendedVersion.Substring(0, recommendedVersionDelimiterEndIndex);
                recommendedVersion = recommendedVersion.Replace(Constants.RECOMMENDED_VERSION_HTML_DELIMITER_START, string.Empty);
                if (IsValidVersionNumber(recommendedVersion)) {
                    result = recommendedVersion;
                }
            }
            return result;
        }

        private static bool IsValidVersionNumber(string candidate) {
            bool result = false;
            candidate = candidate.Replace(".", string.Empty);
            try {
                long.Parse(candidate);
                result = true;
            }
            catch (Exception e) {
                MessagesForm.AddMessage(DateTime.Now, "IVVN: " + e.Message, Constants.MessageType.Error);
            }
            return result;
        }
    }
}