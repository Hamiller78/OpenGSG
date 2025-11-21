using System.Device.Location;

namespace Map
{
    public abstract class Projection
    {
        public abstract Tuple<double, double> GetMapCoordinates(GeoCoordinate globePoint);
        public abstract GeoCoordinate GetGlobeCoordinates(Tuple<double, double> mapPoint);
    }
}
