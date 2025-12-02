namespace OpenGSGLibrary.SystemDeviceLocation
{
    // Minimal replacement for System.Device.Location.GeoCoordinate used by the original VB code.
    // This local shim is used when the official package is unavailable.
    public class GeoCoordinate
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        // Returns distance in meters using haversine formula
        public double GetDistanceTo(GeoCoordinate other)
        {
            const double R = 6371000.0; // Earth radius in meters
            double lat1 = ToRadians(this.Latitude);
            double lat2 = ToRadians(other.Latitude);
            double dLat = ToRadians(other.Latitude - this.Latitude);
            double dLon = ToRadians(other.Longitude - this.Longitude);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double deg) => deg * Math.PI / 180.0;
    }
}
