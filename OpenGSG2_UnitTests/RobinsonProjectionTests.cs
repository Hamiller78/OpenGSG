using System;
using NUnit.Framework;
using Map;
using System.Device.Location;

namespace OpenGSG2_UnitTests
{
    [TestFixture]
    public class RobinsonProjectionTests
    {
        [Test]
        public void Test_MapToGlobalCoordinates()
        {
            var projection = new RobinsonProjection();
            projection.SetCapitalR(715D);

            var mapCoords = Tuple.Create(-714D, 0D);
            var geoCoords = projection.GetGlobeCoordinates(mapCoords);

            Assert.That(geoCoords.Longitude, Is.InRange(-180D, 179D));
            Assert.That(geoCoords.Latitude, Is.InRange(-1D, 1D));
        }
    }
}
