using ColdWarGameLogic.GameLogic;
using OpenGSGLibrary.WorldMap;

namespace ColdWarPrototype.Controller
{
    public class ProvinceEventArgs : EventArgs
    {
        public int ProvinceId { get; }

        public ProvinceEventArgs(int id)
        {
            ProvinceId = id;
        }
    }

    public class CountryEventArgs : EventArgs
    {
        public string CountryTag { get; }

        public CountryEventArgs(string tag)
        {
            CountryTag = tag;
        }
    }

    internal class MouseController
    {
        public event EventHandler<ProvinceEventArgs>? HoveredProvinceChanged;
        public event EventHandler<CountryEventArgs>? HoveredCountryChanged;

        private readonly MasterController gameController_;
        private ProvinceMap? provinceMap_;
        private int currentProvinceId_ = -1;
        private string currentCountryTag_ = string.Empty;
        private double mapScaling_ = 0.0;

        public MouseController(MasterController gameController)
        {
            gameController_ =
                gameController ?? throw new ArgumentNullException(nameof(gameController));
            provinceMap_ = gameController_.GetWorldManager()?.provinceMap;
        }

        public void SetMapScalingFactor(Size guiViewSize, Size mapSize)
        {
            if (guiViewSize.Width <= 0 || guiViewSize.Height <= 0)
                return;
            var xFactor = (double)mapSize.Width / guiViewSize.Width;
            var yFactor = (double)mapSize.Height / guiViewSize.Height;
            mapScaling_ = Math.Max(xFactor, yFactor);
        }

        public void HandleMouseMovedOverMap(MouseEventArgs e)
        {
            if (provinceMap_ == null || e == null)
                return;

            var mapX = (int)(e.X * mapScaling_);
            var mapY = (int)(e.Y * mapScaling_);

            var rgb = provinceMap_.GetPixelRgb(mapX, mapY);
            var provinceId = provinceMap_.GetProvinceNumber(rgb);
            if (provinceId != -1 && provinceId != currentProvinceId_)
            {
                HoveredProvinceChanged?.Invoke(this, new ProvinceEventArgs(provinceId));
                CheckChangedCountry(provinceId);
                currentProvinceId_ = provinceId;
            }
        }

        private void CheckChangedCountry(int provinceId)
        {
            var state = gameController_.tickHandler?.GetState();
            var provTable = state?.GetProvinceTable();
            if (provTable != null && provTable.TryGetValue(provinceId, out var prov))
            {
                var tag = prov.Owner;
                if (!string.Equals(tag, currentCountryTag_, StringComparison.Ordinal))
                {
                    currentCountryTag_ = tag ?? string.Empty;
                    HoveredCountryChanged?.Invoke(this, new CountryEventArgs(currentCountryTag_));
                }
            }
        }
    }
}
