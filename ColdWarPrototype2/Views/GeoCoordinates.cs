using System;
using System.Reflection;
using Simulation;
using WorldData;

namespace ColdWarPrototype2.Views
{
    public class GeoCoordinates
    {
        private readonly MainWindow motherWindow_;
        private int currentProvinceId_ = -1;

        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }

        public GeoCoordinates(MainWindow motherWindow)
        {
            motherWindow_ = motherWindow;
        }

        public void UpdateCurrentCoordinates(WorldState state, int provinceId)
        {
            if (state == null)
                return;
            var table = state.GetProvinceTable();
            if (table == null)
                return;

            if (!table.TryGetValue(provinceId, out var prov))
                return;
            currentProvinceId_ = provinceId;

            // Try common property/field names that might contain coordinates
            Latitude = TryGetDoubleMember(prov, "latitude") ?? TryGetDoubleMember(prov, "lat");
            Longitude =
                TryGetDoubleMember(prov, "longitude")
                ?? TryGetDoubleMember(prov, "lon")
                ?? TryGetDoubleMember(prov, "lng");

            // If coordinates found, apply to form controls (if present)
            TryApplyToFormControl("Latitude", Latitude?.ToString("F6") ?? string.Empty);
            TryApplyToFormControl("Longitude", Longitude?.ToString("F6") ?? string.Empty);
            TryApplyToFormControl(
                "Coordinates",
                (Latitude.HasValue && Longitude.HasValue)
                    ? $"{Latitude:F6}, {Longitude:F6}"
                    : string.Empty
            );
        }

        public void UpdateCurrentCoordinates(WorldState state)
        {
            if (currentProvinceId_ >= 0)
                UpdateCurrentCoordinates(state, currentProvinceId_);
        }

        public void SetCoordinates(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
            TryApplyToFormControl("Latitude", lat.ToString("F6"));
            TryApplyToFormControl("Longitude", lon.ToString("F6"));
            TryApplyToFormControl("Coordinates", $"{lat:F6}, {lon:F6}");
        }

        private double? TryGetDoubleMember(object obj, string name)
        {
            if (obj == null)
                return null;
            var t = obj.GetType();

            // property
            var pi = t.GetProperty(
                name,
                BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.IgnoreCase
            );
            if (pi != null)
            {
                var val = pi.GetValue(obj);
                if (TryConvertToDouble(val, out var d))
                    return d;
            }

            // field
            var fi = t.GetField(
                name,
                BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.IgnoreCase
            );
            if (fi != null)
            {
                var val = fi.GetValue(obj);
                if (TryConvertToDouble(val, out var d))
                    return d;
            }

            return null;
        }

        private bool TryConvertToDouble(object? val, out double result)
        {
            result = 0;
            if (val == null)
                return false;
            if (val is double dd)
            {
                result = dd;
                return true;
            }
            if (val is float f)
            {
                result = f;
                return true;
            }
            if (val is decimal dec)
            {
                result = (double)dec;
                return true;
            }
            if (val is int i)
            {
                result = i;
                return true;
            }
            if (val is long l)
            {
                result = l;
                return true;
            }
            if (double.TryParse(val.ToString(), out var parsed))
            {
                result = parsed;
                return true;
            }
            return false;
        }

        private void TryApplyToFormControl(string controlName, string text)
        {
            if (motherWindow_ == null)
                return;
            var t = motherWindow_.GetType();

            var fi = t.GetField(
                controlName,
                BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.IgnoreCase
            );
            if (fi != null)
            {
                var obj = fi.GetValue(motherWindow_);
                if (obj is System.Windows.Forms.Control c)
                {
                    c.Text = text;
                    return;
                }
            }

            var pi = t.GetProperty(
                controlName,
                BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.IgnoreCase
            );
            if (pi != null)
            {
                var obj = pi.GetValue(motherWindow_);
                if (obj is System.Windows.Forms.Control c)
                {
                    c.Text = text;
                    return;
                }
            }
        }
    }
}
