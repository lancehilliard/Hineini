using System;

namespace Hineini.Location.GPS {
    /// <summary>
    /// Event args used for LocationChanged events.
    /// </summary>
    public class LocationChangedEventArgs : EventArgs {
        private readonly GpsPosition position;

        public LocationChangedEventArgs(GpsPosition position) {
            this.position = position;
        }

        /// <summary>
        /// Gets the new position when the GPS reports a new position.
        /// </summary>
        public GpsPosition Position {
            get { return position; }
        }
    }
}