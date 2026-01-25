using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSGLibrary.WorldMap
{
    /// <summary>
    /// Creates an image where each province is coloured by its owner's country colour.
    /// Uses pre-computed pixel cache and LockBits for maximum performance.
    /// NOTE: Caller should NOT dispose the returned Image - it's managed by this class.
    /// </summary>
    public class CountryModeMapMaker : ModeMapMaker, IDisposable
    {
        private readonly ProvincePixelCache _pixelCache;
        private Bitmap? _cachedBitmap;
        private bool _disposed;

        /// <summary>
        /// Province ID to highlight (blink). -1 for none.
        /// </summary>
        public int HighlightedProvinceId { get; set; } = -1;

        /// <summary>
        /// Whether the highlighted province should be shown in highlight color this frame.
        /// Toggle this to create blinking effect.
        /// </summary>
        public bool ShowHighlight { get; set; } = true;

        /// <summary>
        /// If true, uses original map colors instead of country colors (raw mode with highlighting).
        /// </summary>
        public bool UseRawColors { get; set; } = false;

        public CountryModeMapMaker(ProvinceMap provinceMap)
            : base(provinceMap)
        {
            _pixelCache = new ProvincePixelCache(provinceMap);
        }

        /// <summary>
        /// Generates a country-coloured map image from the given world state.
        /// 10-100x faster than SetPixel approach.
        /// NOTE: Do NOT dispose the returned Image - it's reused across calls.
        /// </summary>
        public override Image MakeMap(WorldState sourceState)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            var width = _pixelCache.Width;
            var height = _pixelCache.Height;

            // Recreate bitmap if size changed or first call
            if (
                _cachedBitmap == null
                || _cachedBitmap.Width != width
                || _cachedBitmap.Height != height
            )
            {
                _cachedBitmap?.Dispose();
                _cachedBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            }

            // Lock both source and destination bitmaps
            var sourceBitmap = _sourceMap.sourceBitmap;
            var sourceBitmapData = sourceBitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            var destBitmapData = _cachedBitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
            );

            try
            {
                int bytes = Math.Abs(destBitmapData.Stride) * height;
                byte[] rgbValues = new byte[bytes];

                // Copy original bitmap as base layer
                Marshal.Copy(sourceBitmapData.Scan0, rgbValues, 0, bytes);

                // If NOT in raw mode, overwrite province pixels with country colors
                if (!UseRawColors)
                {
                    var provinceColors = BuildProvinceColorLookup(sourceState);

                    foreach (var (provinceId, pixels) in GetAllProvincePixels())
                    {
                        Color color = provinceColors.TryGetValue(provinceId, out var c)
                            ? c
                            : Color.AntiqueWhite;

                        // Apply highlight if needed
                        if (provinceId == HighlightedProvinceId && ShowHighlight)
                        {
                            color = Color.FromArgb(
                                (color.R + 255) / 2,
                                (color.G + 255) / 2,
                                (color.B + 255) / 2
                            );
                        }

                        // Fill province pixels
                        FillProvincePixels(rgbValues, pixels, color);
                    }
                }
                else
                {
                    // Raw mode: Only modify highlighted province
                    if (HighlightedProvinceId > 0 && ShowHighlight)
                    {
                        var pixels = _pixelCache.GetProvincePixels(HighlightedProvinceId);
                        if (pixels != null)
                        {
                            // Brighten existing colors
                            foreach (int pixelIndex in pixels)
                            {
                                int offset = pixelIndex * 4;
                                rgbValues[offset] = (byte)Math.Min(255, rgbValues[offset] + 80); // B
                                rgbValues[offset + 1] = (byte)
                                    Math.Min(255, rgbValues[offset + 1] + 80); // G
                                rgbValues[offset + 2] = (byte)
                                    Math.Min(255, rgbValues[offset + 2] + 80); // R
                            }
                        }
                    }
                }

                Marshal.Copy(rgbValues, 0, destBitmapData.Scan0, bytes);
            }
            finally
            {
                _cachedBitmap.UnlockBits(destBitmapData);
                sourceBitmap.UnlockBits(sourceBitmapData);
            }

            return _cachedBitmap;
        }

        // Helper method to reduce duplication
        private void FillProvincePixels(byte[] rgbValues, int[] pixels, Color color)
        {
            byte b = color.B;
            byte g = color.G;
            byte r = color.R;
            byte a = color.A;

            foreach (int pixelIndex in pixels)
            {
                int offset = pixelIndex * 4;
                rgbValues[offset] = b;
                rgbValues[offset + 1] = g;
                rgbValues[offset + 2] = r;
                rgbValues[offset + 3] = a;
            }
        }

        private Dictionary<int, Color> BuildProvinceColorLookup(WorldState sourceState)
        {
            var lookup = new Dictionary<int, Color>();
            var provinceTable = sourceState.GetProvinceTable();
            var countryTable = sourceState.GetCountryTable();

            if (provinceTable == null || countryTable == null)
                return lookup;

            foreach (var (provinceId, province) in provinceTable)
            {
                if (string.IsNullOrEmpty(province.Owner))
                {
                    lookup[provinceId] = Color.AntiqueWhite;
                    continue;
                }

                if (countryTable.TryGetValue(province.Owner, out var country))
                {
                    var (r, g, b) = country.Color;
                    lookup[provinceId] = Color.FromArgb(r, g, b);
                }
                else
                {
                    lookup[provinceId] = Color.AntiqueWhite;
                }
            }

            return lookup;
        }

        private IEnumerable<(int provinceId, int[] pixels)> GetAllProvincePixels()
        {
            // Get province IDs directly from the cache
            foreach (var provinceId in _pixelCache.GetAllProvinceIds())
            {
                var pixels = _pixelCache.GetProvincePixels(provinceId);
                if (pixels != null)
                {
                    yield return (provinceId, pixels);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _cachedBitmap?.Dispose();
                    _cachedBitmap = null;
                }

                _disposed = true;
            }
        }
    }
}
