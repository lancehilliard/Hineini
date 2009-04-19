using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Xml;
using Hineini.Utility;

namespace Hineini.Maps {
    public class MapManager {
        public static string GetMapServiceUrl(MapInfo mapInfo, int mapWidth, int mapHeight) {
            string locationParamString = mapInfo.LocationLatLong == null ? string.Format("location={0}", mapInfo.LocationName) : string.Format("latitude={0}&longitude={1}", mapInfo.LocationLatLong.Latitude, mapInfo.LocationLatLong.Longitude);
            string result = String.Format("http://local.yahooapis.com/MapsService/V1/mapImage?appid={0}&{1}&zoom={2}&image_width={3}&image_height={4}", Constants.YAHOO_MAPS_APPID, locationParamString, mapInfo.MapZoomLevel, mapWidth, mapHeight);
            return result;
        }

        public static string GetMapImageUrl(XmlReader xmlTextReader) {
            string result = String.Empty;
            while (xmlTextReader.Read()) {
                if (XmlNodeType.Element.Equals(xmlTextReader.NodeType) && "Result".Equals(xmlTextReader.Name)) {
                    result = xmlTextReader.ReadString();
                    break;
                }
            }
            return result;
        }

        public static Bitmap GetMapImage(string imageUrl) {
            Bitmap result = null;
            WebRequest webRequest = WebRequest.Create(imageUrl);
            try {
                using (WebResponse webResponse = webRequest.GetResponse()) {
                    using (Stream responseStream = webResponse.GetResponseStream()) {
                        result = new Bitmap(responseStream);
                    }
                }
            }
            catch (Exception e) {
            }
            return result;
        }
    }
}
