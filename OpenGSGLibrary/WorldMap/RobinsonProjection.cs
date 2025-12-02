using OpenGSGLibrary.SystemDeviceLocation;
using System;

namespace OpenGSGLibrary.WorldMap
{
    public class RobinsonProjection : Projection
    {
        private const double centralMeridian = 0.0;
        private double _capitalR = 1D;

        public override Tuple<double, double> GetMapCoordinates(GeoCoordinate globePoint)
        {
            var capitalXY = GetXY(globePoint.Latitude);
            var x = 3D / 8D * 0.8487 * GetCapitalR() * capitalXY.Item1 * (globePoint.Longitude - centralMeridian) * Math.PI / 180D;
            var y = 3D / 8D * 1.3523 * GetCapitalR() * capitalXY.Item2;
            return Tuple.Create(x, y);
        }

        public override GeoCoordinate GetGlobeCoordinates(Tuple<double, double> mapPoint)
        {
            double latitude = 0D;
            double longitude = 0D;

            latitude = -GetLatitudeOfY(mapPoint.Item2 / (3D / 8D * 1.3523 * GetCapitalR()));
            longitude = mapPoint.Item1 / (3D / 8D * 0.8487 * GetCapitalR() * GetXY(Math.Abs(latitude)).Item1) * 180D / Math.PI + centralMeridian;

            latitude = latitude % 180D;
            longitude = longitude % 180D;

            return new GeoCoordinate(latitude, longitude);
        }

        public double GetCapitalR() => _capitalR;
        public void SetCapitalR(double value) => _capitalR = value;

        private Tuple<double, double> GetXY(double latitude)
        {
            double capitalX = 0, capitalY = 0;
            if (0 <= latitude && latitude < 5)
            {
                capitalX = LinearInterpolation(latitude, 0, 1, 5, 0.9986);
                capitalY = LinearInterpolation(latitude, 0, 0.0, 5, 0.062);
            }
            else if (5 <= latitude && latitude < 10)
            {
                capitalX = LinearInterpolation(latitude, 5, 0.9986, 10, 0.9954);
                capitalY = LinearInterpolation(latitude, 5, 0.062, 10, 0.124);
            }
            else if (10 <= latitude && latitude < 15)
            {
                capitalX = LinearInterpolation(latitude, 10, 0.9954, 15, 0.99);
                capitalY = LinearInterpolation(latitude, 10, 0.124, 15, 0.186);
            }
            else if (15 <= latitude && latitude < 20)
            {
                capitalX = LinearInterpolation(latitude, 15, 0.99, 20, 0.9822);
                capitalY = LinearInterpolation(latitude, 15, 0.186, 20, 0.248);
            }
            else if (20 <= latitude && latitude < 25)
            {
                capitalX = LinearInterpolation(latitude, 20, 0.9822, 25, 0.973);
                capitalY = LinearInterpolation(latitude, 20, 0.248, 25, 0.31);
            }
            else if (25 <= latitude && latitude < 30)
            {
                capitalX = LinearInterpolation(latitude, 25, 0.973, 30, 0.96);
                capitalY = LinearInterpolation(latitude, 25, 0.31, 30, 0.372);
            }
            else if (30 <= latitude && latitude < 35)
            {
                capitalX = LinearInterpolation(latitude, 30, 0.96, 35, 0.9427);
                capitalY = LinearInterpolation(latitude, 30, 0.372, 35, 0.434);
            }
            else if (35 <= latitude && latitude < 40)
            {
                capitalX = LinearInterpolation(latitude, 35, 0.9427, 40, 0.9216);
                capitalY = LinearInterpolation(latitude, 35, 0.434, 40, 0.4958);
            }
            else if (40 <= latitude && latitude < 45)
            {
                capitalX = LinearInterpolation(latitude, 40, 0.9216, 45, 0.8962);
                capitalY = LinearInterpolation(latitude, 40, 0.4958, 45, 0.5571);
            }
            else if (45 <= latitude && latitude < 50)
            {
                capitalX = LinearInterpolation(latitude, 45, 0.8962, 50, 0.8679);
                capitalY = LinearInterpolation(latitude, 45, 0.5571, 50, 0.6176);
            }
            else if (50 <= latitude && latitude < 55)
            {
                capitalX = LinearInterpolation(latitude, 50, 0.8679, 55, 0.835);
                capitalY = LinearInterpolation(latitude, 50, 0.6176, 55, 0.6769);
            }
            else if (55 <= latitude && latitude < 60)
            {
                capitalX = LinearInterpolation(latitude, 55, 0.835, 60, 0.7986);
                capitalY = LinearInterpolation(latitude, 55, 0.6769, 60, 0.7346);
            }
            else if (60 <= latitude && latitude < 65)
            {
                capitalX = LinearInterpolation(latitude, 60, 0.7986, 65, 0.7597);
                capitalY = LinearInterpolation(latitude, 60, 0.7346, 65, 0.7903);
            }
            else if (65 <= latitude && latitude < 70)
            {
                capitalX = LinearInterpolation(latitude, 65, 0.7597, 70, 0.7186);
                capitalY = LinearInterpolation(latitude, 65, 0.7903, 70, 0.8435);
            }
            else if (70 <= latitude && latitude < 75)
            {
                capitalX = LinearInterpolation(latitude, 70, 0.7186, 75, 0.6732);
                capitalY = LinearInterpolation(latitude, 70, 0.8435, 75, 0.8936);
            }
            else if (75 <= latitude && latitude < 80)
            {
                capitalX = LinearInterpolation(latitude, 75, 0.6732, 80, 0.6213);
                capitalY = LinearInterpolation(latitude, 75, 0.8936, 80, 0.9394);
            }
            else if (80 <= latitude && latitude < 85)
            {
                capitalX = LinearInterpolation(latitude, 80, 0.6213, 85, 0.5722);
                capitalY = LinearInterpolation(latitude, 80, 0.9394, 85, 0.9761);
            }
            else if (85 <= latitude && latitude <= 90)
            {
                capitalX = LinearInterpolation(latitude, 85, 0.5722, 90, 0.5322);
                capitalY = LinearInterpolation(latitude, 85, 0.9761, 90, 1);
            }
            return Tuple.Create(capitalX, capitalY);
        }

        private double GetLatitudeOfY(double capitalY)
        {
            double latitude = 0D;
            var abs = Math.Abs(capitalY);

            if (0 <= abs && abs < 0.062) latitude = LinearInterpolation(abs, 0, 0, 0.062, 5);
            else if (0.062 <= abs && abs < 0.124) latitude = LinearInterpolation(abs, 0.062, 5, 0.124, 10);
            else if (0.124 <= abs && abs < 0.186) latitude = LinearInterpolation(abs, 0.124, 10, 0.186, 15);
            else if (0.186 <= abs && abs < 0.248) latitude = LinearInterpolation(abs, 0.186, 15, 0.248, 20);
            else if (0.248 <= abs && abs < 0.31) latitude = LinearInterpolation(abs, 0.248, 20, 0.31, 25);
            else if (0.31 <= abs && abs < 0.372) latitude = LinearInterpolation(abs, 0.31, 25, 0.372, 30);
            else if (0.372 <= abs && abs < 0.434) latitude = LinearInterpolation(abs, 0.372, 30, 0.434, 35);
            else if (0.434 <= abs && abs < 0.4958) latitude = LinearInterpolation(abs, 0.434, 35, 0.4958, 40);
            else if (0.4958 <= abs && abs < 0.5571) latitude = LinearInterpolation(abs, 0.4958, 40, 0.5571, 45);
            else if (0.5571 <= abs && abs < 0.6176) latitude = LinearInterpolation(abs, 0.5571, 45, 0.6176, 50);
            else if (0.6176 <= abs && abs < 0.6769) latitude = LinearInterpolation(abs, 0.6176, 50, 0.6769, 55);
            else if (0.6769 <= abs && abs < 0.7346) latitude = LinearInterpolation(abs, 0.6769, 55, 0.7346, 60);
            else if (0.7346 <= abs && abs < 0.7903) latitude = LinearInterpolation(abs, 0.7346, 60, 0.7903, 65);
            else if (0.7903 <= abs && abs < 0.8435) latitude = LinearInterpolation(abs, 0.7903, 65, 0.8435, 70);
            else if (0.8435 <= abs && abs < 0.8936) latitude = LinearInterpolation(abs, 0.8435, 70, 0.8936, 75);
            else if (0.8936 <= abs && abs < 0.9394) latitude = LinearInterpolation(abs, 0.8936, 75, 0.9394, 80);
            else if (0.9394 <= abs && abs < 0.9761) latitude = LinearInterpolation(abs, 0.9394, 80, 0.9761, 85);
            else if (0.9761 <= abs && abs <= 1.0) latitude = LinearInterpolation(abs, 0.9761, 85, 1.0, 90);

            if (capitalY < 0) latitude = -latitude;
            return latitude;
        }

        private double LinearInterpolation(double x, double x1, double y1, double x2, double y2)
        {
            var deltaX = (x - x1) / (x2 - x1);
            return (y2 - y1) * deltaX + y1;
        }
    }
}
