using OpenGSGLibrary.SystemDeviceLocation;
using OpenGSGLibrary.WorldMap;

namespace OpenGSG_UnitTests
{
    [TestFixture]
    public class NavigatorTests
    {
        [Test]
        public void Test_CompareDistanceMethods()
        {
            // Coordinates London and New York according to Google
            double latitudeLondon = 51.5074D;
            double longitudeLondon = -0.1278D;
            double latitudeNewYork = 40.7128D;
            double longitudeNewYork = -74.006D;
            var coordinatesLondon = new GeoCoordinate(latitudeLondon, longitudeLondon);
            var coordinatesNewYork = new GeoCoordinate(latitudeNewYork, longitudeNewYork);

            // Distance London to New York is ~5567km, allow 10km deviation
            double distanceMethod1 = Navigator.GetDistanceInKm(
                latitudeLondon,
                longitudeLondon,
                latitudeNewYork,
                longitudeNewYork
            );
            Assert.That(Math.Abs(5567D - distanceMethod1), Is.LessThan(10D));

            double distanceMethod2 = Navigator.GetDistanceInKm(
                coordinatesLondon,
                coordinatesNewYork
            );
            Assert.That(Math.Abs(5567D - distanceMethod2), Is.LessThan(10D));
        }
    }
}
