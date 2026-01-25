namespace OpenGSGLibrary.WorldMap
{
    /// <summary>
    /// Caches which pixels belong to each province for fast rendering.
    /// Built once at startup, reused for all map renders.
    /// </summary>
    public class ProvincePixelCache
    {
        /// <summary>
        /// Maps province ID to flat array of pixel indices (y * width + x).
        /// Much faster than List<Point> for iteration.
        /// </summary>
        private readonly Dictionary<int, int[]> _provincePixels;
        private readonly int _width;
        private readonly int _height;

        public ProvincePixelCache(ProvinceMap provinceMap)
        {
            _width = provinceMap.sourceBitmap.Width;
            _height = provinceMap.sourceBitmap.Height;
            _provincePixels = BuildPixelCache(provinceMap);
        }

        private Dictionary<int, int[]> BuildPixelCache(ProvinceMap provinceMap)
        {
            // First pass: count pixels per province
            var pixelCounts = new Dictionary<int, int>();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var rgb = provinceMap.GetPixelRgb(x, y);
                    var provinceId = provinceMap.GetProvinceNumber(rgb);

                    if (provinceId != -1)
                    {
                        pixelCounts.TryGetValue(provinceId, out int count);
                        pixelCounts[provinceId] = count + 1;
                    }
                }
            }

            // Second pass: allocate arrays and fill
            var provincePixels = new Dictionary<int, int[]>();
            var indices = new Dictionary<int, int>(); // Current index per province

            foreach (var kvp in pixelCounts)
            {
                provincePixels[kvp.Key] = new int[kvp.Value];
                indices[kvp.Key] = 0;
            }

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var rgb = provinceMap.GetPixelRgb(x, y);
                    var provinceId = provinceMap.GetProvinceNumber(rgb);

                    if (provinceId != -1)
                    {
                        int pixelIndex = y * _width + x;
                        int idx = indices[provinceId];
                        provincePixels[provinceId][idx] = pixelIndex;
                        indices[provinceId] = idx + 1;
                    }
                }
            }

            return provincePixels;
        }

        /// <summary>
        /// Gets flat pixel indices for a province.
        /// </summary>
        public int[]? GetProvincePixels(int provinceId)
        {
            return _provincePixels.TryGetValue(provinceId, out var pixels) ? pixels : null;
        }

        /// <summary>
        /// Gets all province IDs that have cached pixels.
        /// </summary>
        public IEnumerable<int> GetAllProvinceIds()
        {
            return _provincePixels.Keys;
        }

        public int Width => _width;
        public int Height => _height;
    }
}
