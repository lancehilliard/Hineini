using System.Drawing;
using System.IO;
using System.Net;
using Hineini.Utility;

namespace Hineini.Maps {
    public class MapManager {
        public static Bitmap GetMapImage(string imageUrl) {
            Helpers.WriteToExtraLog("Getting map image: " + imageUrl, null);
            Bitmap result;
            WebRequest webRequest = WebRequest.Create(imageUrl);
            using (WebResponse webResponse = webRequest.GetResponse()) {
                using (Stream responseStream = webResponse.GetResponseStream()) {
                    result = new Bitmap(responseStream);
                }
            }
            return result;
        }
    }
}