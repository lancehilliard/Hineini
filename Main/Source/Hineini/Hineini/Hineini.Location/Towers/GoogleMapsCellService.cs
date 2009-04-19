using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Hineini.FireEagle;
using Hineini.Location.Towers;

namespace Hineini.Location.Towers {
    /// <summary>
    /// Submits cell tower data to Google for converstion to Latitude/Longitude pairs
    /// </summary>
    public class GoogleMapsCellService
    {
        private const string Google_Mobile_Service_Uri = "http://www.google.com/glm/mmap";

        /// <summary>
        /// Gets the current latitude and longitude based on cell tower data.
        /// </summary>
        /// <returns></returns>
        public static FireEagle.Position GetLocation(CellTower tower)
        {
            try
            {
                // Translate cell tower data into http post parameter data
                byte[] formData = GetFormPostData(tower.cellid, 
                                                  tower.mcc, 
                                                  tower.mcc, 
                                                  tower.lac);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(Google_Mobile_Service_Uri));
                request.Method = "POST";

                request.ContentLength = formData.Length;
                request.ContentType = "application/binary";
                Stream outputStream = request.GetRequestStream();

                // Write the cell data to the http stream
                outputStream.Write(formData, 0, formData.Length);
                outputStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                return ReadResponse(response);
            }
            catch
            {}

            return new Position();
        }
        /// <summary>
        /// Reads the response data and converts it into a latitude/longitude pair.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>current latitude and longitude</returns>
        private static FireEagle.Position ReadResponse(HttpWebResponse response)
        {
            byte[] responseData = new byte[response.ContentLength];
            int bytesRead = 0;

            // Read the response into the response byte array
            while (bytesRead < responseData.Length)
            {
                bytesRead += response.GetResponseStream().Read(responseData, bytesRead, responseData.Length - bytesRead);
            }

            // Check the response
            if (response.StatusCode == HttpStatusCode.OK)
            {
                int successful = Convert.ToInt32(GetCode(responseData, 3));

                if (successful == 0)
                {
                    return new Position
                           {
                               Latitude = GetCode(responseData, 7) / 1000000,
                               Longitude = GetCode(responseData, 11) / 1000000
                           };
                }
            }

            return new Position();
        }
        /// <summary>
        /// Gets the form post data.
        /// </summary>
        /// <param name="cellTowerId">The cell tower id.</param>
        /// <param name="mobileCountryCode">The mobile country code.</param>
        /// <param name="mobileNetworkCode">The mobile network code.</param>
        /// <param name="locationAreaCode">The location area code.</param>
        /// <returns></returns>
        private static byte[] GetFormPostData(int cellTowerId, int mobileCountryCode, int mobileNetworkCode, int locationAreaCode) //, bool shortCID
        {
            byte[] pd = new byte[55];
            pd[1] = 14;     //0x0e;
            pd[16] = 27;    //0x1b;
            pd[47] = 255;   //0xff;
            pd[48] = 255;   //0xff;
            pd[49] = 255;   //0xff;
            pd[50] = 255;   //0xff;

            pd[28] = ((Int64)cellTowerId > 65536) ? (byte)5 : (byte)3; // GSM uses 4 digits while UTMS used 6 digits (hex)

            Shift(pd, 17, mobileNetworkCode);
            Shift(pd, 21, mobileCountryCode);
            Shift(pd, 31, cellTowerId);
            Shift(pd, 35, locationAreaCode);
            Shift(pd, 39, mobileNetworkCode);
            Shift(pd, 43, mobileCountryCode);

            return pd;
        }
        /// <summary>
        /// Gets the code from the byte array.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        private static double GetCode(byte[] data, int startIndex)
        {
            return ((double)((data[startIndex++] << 24) |
                             (data[startIndex++] << 16) |
                             (data[startIndex++] << 8) |
                             (data[startIndex++])));
        }
        /// <summary>
        /// Shifts specified data in the byte array starting at the specified array index.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="leftOperand">The left operand.</param>
        private static void Shift(byte[] data, int startIndex, int leftOperand)
        {
            int rightOperand = 24;
            
            for (int i = 0; i < 4; i++, rightOperand -= 8)
            {
                data[startIndex++] = (byte)((leftOperand >> rightOperand) & 255);
            }
        }
    }
}