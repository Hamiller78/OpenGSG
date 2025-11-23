using System;
using System.Drawing;
using System.Windows.Forms;
using Simulation;
using Map;

namespace ColdWarPrototype2.Gui
{
    public class ProvinceEventArgs : EventArgs
    {
        public int ProvinceId { get; }
        public ProvinceEventArgs(int id) { ProvinceId = id; }
    }

    public class CountryEventArgs : EventArgs
    {
        public string CountryTag { get; }
        public CountryEventArgs(string tag) { CountryTag = tag; }
    }

    public class MouseController
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
            gameController_ = gameController;
            provinceMap_ = gameController_.GetWorldManager().provinceMap;
        }

        public void SetMapScalingFactor(Size guiViewSize, Size mapSize)
        {
            var xFactor = (double)mapSize.Width / guiViewSize.Width;
            var yFactor = (double)mapSize.Height / guiViewSize.Height;
            mapScaling_ = Math.Max(xFactor, yFactor);
        }

        public void HandleMouseMovedOverMap(MouseEventArgs e)
        {
            if (provinceMap_ == null) return;
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
            var provTable = gameController_.tickHandler.GetState().GetProvinceTable();
            if (provTable != null && provTable.TryGetValue(provinceId, out var prov))
            {
                var tag = prov.GetOwner();
                if (tag != currentCountryTag_)
                {
                    currentCountryTag_ = tag;
                    HoveredCountryChanged?.Invoke(this, new CountryEventArgs(tag));
                }
            }
        }
    }
}
