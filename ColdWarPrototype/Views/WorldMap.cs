using ColdWarPrototype2;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.WorldMap;

namespace ColdWarPrototype.Views
{
    public class WorldMap
    {
        private readonly MainWindow motherWindow_; // Make readonly
        private ProvinceMap provinceMap_ = default!;
        private double mapScaling_;

        public WorldMap(MainWindow motherWindow)
        {
            motherWindow_ = motherWindow;
        }

        public void SetSourceProvinceMap(ProvinceMap newProvinceMap)
        {
            provinceMap_ = newProvinceMap;

            // Create map maker once
            MapMaker = new CountryModeMapMaker(provinceMap_);

            SetMapPicture();
        }

        public void UpdateCountryMap(WorldState currentState)
        {
            if (provinceMap_ == null || MapMaker == null)
                return;

            // Render the country map (updates internal cached bitmap)
            MapMaker.MakeMap(currentState);

            // Refresh display (works in both raw and political modes now)
            SetMapPicture();
        }

        public void SetMapPicture()
        {
            if (provinceMap_ == null || MapMaker == null)
                return;

            // Set raw mode flag based on current map mode
            if (MapMaker is CountryModeMapMaker countryMap)
            {
                countryMap.UseRawColors = !motherWindow_.MapModePolitical.Checked;
            }

            // Always render through MapMaker (handles both modes now)
            var renderedBitmap = MapMaker.MakeMap(
                motherWindow_.GetGameController().TickHandler.GetState()
            );

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

        public ModeMapMaker? MapMaker { get; private set; }
    }
}
