using OpenGSGLibrary.SystemDeviceLocation;

namespace OpenGSGLibrary.WorldMap
{
    /// <summary>
    /// Base projection for converting between globe coordinates (latitude/longitude)
    /// and 2D map coordinates used by the rendering system.
    /// Implementations should provide forward and inverse transformations.
    /// </summary>
    public abstract class Projection
    {
        /// <summary>
        /// Converts a geographic coordinate to map (x,y) coordinates.
        /// </summary>
        /// <param name="globePoint">Geographic point (latitude/longitude).</param>
        /// <returns>Tuple with map X and Y coordinates.</returns>
        public abstract Tuple<double, double> GetMapCoordinates(GeoCoordinate globePoint);

        /// <summary>
        /// Converts map (x,y) coordinates back to geographic coordinates.
        /// </summary>
        /// <param name="mapPoint">Tuple with map X and Y coordinates.</param>
        /// <returns>Geographic coordinate (latitude/longitude).</returns>
        public abstract GeoCoordinate GetGlobeCoordinates(Tuple<double, double> mapPoint);
    }
}
