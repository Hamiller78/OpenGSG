using ColdWarGameLogic.GameLogic;
using OpenGSGLibrary.WorldMap;

namespace ColdWarPrototype.Controller
{
    public class ProvinceEventArgs(int id) : EventArgs
    {
        public int ProvinceId { get; } = id;
    }

    public class CountryEventArgs(string tag) : EventArgs
    {
        public string CountryTag { get; } = tag;
    }

    internal class MouseController
    {
        public event EventHandler<ProvinceEventArgs>? HoveredProvinceChanged;
        public event EventHandler<CountryEventArgs>? HoveredCountryChanged;

        private readonly MasterController _gameController;
        private readonly ProvinceMap _provinceMap;
        private int _currentProvinceId = -1;
        private string _currentCountryTag = string.Empty;
        private double _mapScaling = 0.0;

        public MouseController(MasterController gameController)
        {
            _gameController =
                gameController ?? throw new ArgumentNullException(nameof(gameController));
            _provinceMap = _gameController.WorldData.ProvinceMap;
        }

        public void SetMapScalingFactor(Size guiViewSize, Size mapSize)
        {
            if (guiViewSize.Width <= 0 || guiViewSize.Height <= 0)
                return;
            var xFactor = (double)mapSize.Width / guiViewSize.Width;
            var yFactor = (double)mapSize.Height / guiViewSize.Height;
            _mapScaling = Math.Max(xFactor, yFactor);
        }

        public void HandleMouseMovedOverMap(MouseEventArgs e)
        {
            if (_provinceMap == null || e == null)
                return;

            var mapX = (int)(e.X * _mapScaling);
            var mapY = (int)(e.Y * _mapScaling);

            var rgb = _provinceMap.GetPixelRgb(mapX, mapY);
            var provinceId = _provinceMap.GetProvinceNumber(rgb);
            if (provinceId != -1 && provinceId != _currentProvinceId)
            {
                HoveredProvinceChanged?.Invoke(this, new ProvinceEventArgs(provinceId));
                CheckChangedCountry(provinceId);
                _currentProvinceId = provinceId;
            }
        }

        private void CheckChangedCountry(int provinceId)
        {
            var state = _gameController.TickHandler.GetState();
            var provTable = state?.GetProvinceTable();
            if (provTable != null && provTable.TryGetValue(provinceId, out var prov))
            {
                var tag = prov.Owner;
                if (!string.Equals(tag, _currentCountryTag, StringComparison.Ordinal))
                {
                    _currentCountryTag = tag ?? string.Empty;
                    HoveredCountryChanged?.Invoke(this, new CountryEventArgs(_currentCountryTag));
                }
            }
        }
    }
}
