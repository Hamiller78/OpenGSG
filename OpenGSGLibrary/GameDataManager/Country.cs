using System.Drawing;
using OpenGSGLibrary.Diplomacy;

namespace OpenGSGLibrary.GameDataManager
{
    /// <summary>
    /// Base class for countries. Handles the most basic country properties.
    /// </summary>
    public class Country : GameObject
    {
        // Bitmap for the country's flag. Nullable to satisfy nullable reference types.
        public Bitmap? Flag { get; set; } = default;
        public string Tag { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public List<Province> Provinces { get; } = new();
        public IEconomy Economy { get; set; } = default!;
        public Tuple<byte, byte, byte> Color { get; private set; } = default!;

        /// <summary>
        /// List of diplomatic relations this country has with other countries.
        /// </summary>
        public List<DiplomaticRelation> DiplomaticRelations { get; } = new();

        /// <summary>
        /// Dictionary of opinions this country has toward other countries.
        /// Key: target country tag, Value: Opinion object
        /// </summary>
        public Dictionary<string, Opinion> Opinions { get; } = new();

        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);

            // Only set name from filename if not already set (for initial load from common/countries)
            if (string.IsNullOrEmpty(Name))
            {
                Name = Path.GetFileNameWithoutExtension(fileName);
            }

            // Only set Tag if present in the file (for initial load from common/countries)
            if (parsedData.Contains("tag"))
            {
                Tag = parsedData["tag"].Single()?.ToString() ?? string.Empty;
            }

            // Only set Color if present in the file (for initial load from common/countries)
            if (parsedData.Contains("color"))
            {
                var colorList = (List<int>)parsedData["color"].Single();
                if (colorList != null && colorList.Count >= 3)
                {
                    byte rValue = (byte)colorList[0];
                    byte gValue = (byte)colorList[1];
                    byte bValue = (byte)colorList[2];
                    Color = new Tuple<byte, byte, byte>(rValue, gValue, bValue);
                }
            }

            // Load opinions (generic feature for all GSG games)
            LoadOpinions(parsedData);
        }

        /// <summary>
        /// Loads opinion values from parsed country data.
        /// Format: add_opinion = { target = TAG value = -50 }
        /// </summary>
        protected virtual void LoadOpinions(ILookup<string, object> parsedData)
        {
            if (!parsedData.Contains("add_opinion"))
                return;

            foreach (var opinionData in parsedData["add_opinion"])
            {
                if (opinionData is ILookup<string, object> opinionLookup)
                {
                    if (!opinionLookup.Contains("target") || !opinionLookup.Contains("value"))
                        continue;

                    var targetTag = opinionLookup["target"].Single()?.ToString();
                    var valueStr = opinionLookup["value"].Single()?.ToString();

                    if (!string.IsNullOrEmpty(targetTag) && int.TryParse(valueStr, out var value))
                    {
                        SetOpinion(targetTag, value);
                    }
                }
            }
        }

        public virtual void LoadFlags(string flagPath)
        {
            var path = Path.Combine(flagPath, Tag + ".png");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Flag image not found: " + path);
            }

            // Load bitmap now that System.Drawing.Common is referenced by the project.
            var img = Image.FromFile(path);
            Flag = new Bitmap(img);
        }

        public void UpdateProvinces(WorldState state)
        {
            Provinces.Clear();
            var provinceTable = state.GetProvinceTable();
            Provinces.AddRange(provinceTable.Values.Where(p => p.Owner == Tag));
        }

        /// <summary>
        /// Checks if this country is at war with the specified country.
        /// </summary>
        public bool IsAtWarWith(string countryTag, DateTime currentDate)
        {
            return DiplomaticRelations
                .OfType<WarRelation>()
                .Any(w => w.ToCountryTag == countryTag && w.IsActive(currentDate));
        }

        /// <summary>
        /// Checks if this country has any active wars.
        /// </summary>
        public bool IsAtWar(DateTime currentDate)
        {
            return DiplomaticRelations.OfType<WarRelation>().Any(w => w.IsActive(currentDate));
        }

        /// <summary>
        /// Gets all diplomatic relations of a specific type.
        /// </summary>
        public IEnumerable<T> GetRelationsOfType<T>(DateTime currentDate)
            where T : DiplomaticRelation
        {
            return DiplomaticRelations.OfType<T>().Where(r => r.IsActive(currentDate));
        }

        /// <summary>
        /// Gets this country's opinion of another country.
        /// Returns 0 if no opinion is se   t.
        /// </summary>
        public int GetOpinionOf(string countryTag, DateTime currentDate)
        {
            if (Opinions.TryGetValue(countryTag, out var opinion))
            {
                return opinion.GetTotalOpinion(currentDate);
            }
            return 0; // Default neutral opinion
        }

        /// <summary>
        /// Sets or updates the base opinion toward another country.
        /// </summary>
        public void SetOpinion(string towardCountryTag, int baseValue)
        {
            if (!Opinions.ContainsKey(towardCountryTag))
            {
                Opinions[towardCountryTag] = new Opinion
                {
                    FromCountryTag = this.Tag,
                    TowardCountryTag = towardCountryTag,
                };
            }

            Opinions[towardCountryTag].BaseValue = Math.Clamp(baseValue, -100, 100);
        }
    }
}
