using OpenGSGLibrary.SystemDeviceLocation;
using System;

namespace OpenGSGLibrary.WorldMap
{
    /// <summary>
    /// Helper methods for geographic navigation and distance calculations.
    /// </summary>
    public static class Navigator
    {
        private const double EarthRadiusInKm = 6371D;

        /// <summary>
        /// Calculates great-circle distance between two points specified as lat/long in degrees.
        /// </summary>
        public static double GetDistanceInKm(double firstLatitude, double firstLongitude, double secondLatitude, double secondLongitude)
        {
            var latitude1 = firstLatitude * Math.PI / 180.0;
            var longitude1 = firstLongitude * Math.PI / 180.0;
            var latitude2 = secondLatitude * Math.PI / 180.0;
            var longitude2 = secondLongitude * Math.PI / 180.0;

            var distanceAngleRad = Math.Acos(
                Math.Sin(latitude1) * Math.Sin(latitude2) +
                Math.Cos(latitude1) * Math.Cos(latitude2) * Math.Cos(longitude2 - longitude1)
            );

            return Math.Abs(distanceAngleRad * EarthRadiusInKm);
        }

        /// <summary>
        /// Convenience wrapper using System.Device.Location.GeoCoordinate.
        /// </summary>
        public static double GetDistanceInKm(GeoCoordinate firstLocation, GeoCoordinate secondLocation)
        {
            return firstLocation.GetDistanceTo(secondLocation) / 1000D;
        }
    }
}
