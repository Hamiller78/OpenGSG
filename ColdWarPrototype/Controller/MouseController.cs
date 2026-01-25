using ColdWarGameLogic.GameLogic;
using OpenGSGLibrary.WorldMap;

namespace ColdWarPrototype.Controller // Changed: uppercase W
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
        /// <summary>
        /// Raised when a province is clicked.
        /// </summary>
        public event EventHandler<ProvinceEventArgs>? ProvinceClicked;

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

        /// <summary>
        /// Gets the province ID at the given mouse position.
        /// Returns -1 if no valid province is found.
        /// </summary>
        private int GetProvinceIdAtMousePosition(MouseEventArgs e)
        {
            if (_provinceMap == null || e == null || _mapScaling == 0)
                return -1;

            // Scale mouse coordinates to original map size
            var mapX = (int)(e.X * _mapScaling);
            var mapY = (int)(e.Y * _mapScaling);

            // Get province ID from pixel color
            var rgb = _provinceMap.GetPixelRgb(mapX, mapY);
            return _provinceMap.GetProvinceNumber(rgb);
        }

        public void HandleMouseMovedOverMap(MouseEventArgs e)
        {
            var provinceId = GetProvinceIdAtMousePosition(e);

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

        /// <summary>
        /// Handles mouse click on the map.
        /// Call this from MapPictureBox.MouseClick event.
        /// </summary>
        public void HandleMouseClickOnMap(MouseEventArgs e)
        {
            var provinceId = GetProvinceIdAtMousePosition(e);

            if (provinceId != -1)
            {
                // Always invoke - even if same province (needed for unpin behavior)
                ProvinceClicked?.Invoke(this, new ProvinceEventArgs(provinceId));
            }
        }
    }
}
