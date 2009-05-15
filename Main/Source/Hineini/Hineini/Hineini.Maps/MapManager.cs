using System.Drawing;
using System.IO;
using System.Net;

namespace Hineini.Maps {
    public class MapManager {
        public static Bitmap GetMapImage(string imageUrl) {
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