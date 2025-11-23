using System.Drawing;
using System.Windows.Forms;
using Map;
using Simulation;
using WorldData;

namespace ColdWarPrototype2.Views
{
    public class WorldMapView
    {
        private MainWindow motherWindow_;
        private ProvinceMap provinceMap_;
        private Image? countryModeMap_;
        private double mapScaling_;

        public WorldMapView(MainWindow motherWindow)
        {
            motherWindow_ = motherWindow;
        }

        public void SetSourceProvinceMap(ProvinceMap newProvinceMap)
        {
            provinceMap_ = newProvinceMap;
            SetMapPicture();
        }

        public void UpdateCountryMap(WorldData.WorldState currentState)
        {
            var renderer = new CountryModeMapMaker(provinceMap_);
            countryModeMap_ = renderer.MakeMap(currentState);
        }

        public void SetMapPicture()
        {
            if (provinceMap_ == null)
                return;

            Image renderedBitmap = motherWindow_.MapModePolitical.Checked
                ? countryModeMap_!
                : provinceMap_.sourceBitmap!;

            var sourceSize = provinceMap_.sourceBitmap!.Size;
            SetMapScalingFactor(motherWindow_.MapPictureBox.Size, sourceSize);
            var newSize = new Size(
                (int)(sourceSize.Width / mapScaling_),
                (int)(sourceSize.Height / mapScaling_)
            );
            var resizedBitmap = new Bitmap(renderedBitmap, newSize);

            motherWindow_.MapPictureBox.Image = resizedBitmap;
            motherWindow_.MapPictureBox.Invalidate();
        }

        private void SetMapScalingFactor(Size newSize, Size originalSize)
        {
            var xFactor = (double)originalSize.Width / newSize.Width;
            var yFactor = (double)originalSize.Height / newSize.Height;
            mapScaling_ = Math.Max(xFactor, yFactor);
        }
    }
}
