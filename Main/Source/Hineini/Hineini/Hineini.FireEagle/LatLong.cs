using System;
using System.Collections.Generic;
using System.Text;

namespace Hineini.FireEagle {
    /// <summary>
    /// Class to represent a latitude and longitude pair
    /// </summary>
    public class LatLong
    {
        /// <summary>
        /// Latitude
        /// </summary>
        private double latitude;

        /// <summary>
        /// Longitude
        /// </summary>
        private double longitude;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Lobgitude</param>
        public LatLong(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        /// <summary>
        /// Gets the latitude
        /// </summary>
        public double Latitude
        {
            get
            {
                return this.latitude;
            }
        }

        /// <summary>
        /// Gets the longitude
        /// </summary>
        public double Longitude
        {
            get
            {
                return this.longitude;
            }
        }

        /// <summary>
        /// Is the LatLong valid?
        /// </summary>
        /// <returns>True if the latitude and longitude are in a valid range</returns>
        public bool Valid()
        {
            return (this.latitude >= -90.0 && this.latitude <= 90.0 && this.longitude >= -180.0 && this.longitude <= 180.0);
        }

        public override string ToString() {
            return string.Format("Latitude: {0}, Longitude: {1}", latitude, longitude);
        }
    }
}