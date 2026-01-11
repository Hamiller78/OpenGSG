using ColdWarGameLogic.Diplomacy;
using OpenGSGLibrary.Diplomacy;
using OpenGSGLibrary.GameDataManager;

namespace ColdWarGameLogic.GameWorld
{
    /// <summary>
    /// Country class for cold war game. Adds specialised properties to base country class.
    /// </summary>
    public class CwpCountry : Country
    {
        public string FullName { get; set; } = string.Empty;
        public string Government { get; set; } = string.Empty;
        public string Allegiance { get; set; } = string.Empty;
        public string Leader { get; set; } = string.Empty;

        public CwpCountry()
            : base() { }

        /// <summary>
        /// Sets the country properties from the parsed data.
        /// </summary>
        /// <param name="fileName">Name of the source file of country object.</param>
        /// <param name="parsedData">Object with the parsed data from that file.</param>
        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);

            // Only load historical data if present (supports split loading from common + history)
            if (parsedData.Contains("long_name"))
            {
                FullName = parsedData["long_name"].Single()?.ToString() ?? string.Empty;
            }

            if (parsedData.Contains("government"))
            {
                Government = parsedData["government"].Single()?.ToString() ?? string.Empty;
            }

            if (parsedData.Contains("allegiance"))
            {
                Allegiance = parsedData["allegiance"].Single()?.ToString() ?? string.Empty;
            }

            if (parsedData.Contains("leader"))
            {
                Leader = parsedData["leader"].Single()?.ToString() ?? string.Empty;
            }

            // Load diplomatic relations
            LoadDiplomaticRelations(parsedData);

            // Initialize economy if not already present
            Economy ??= new CwpEconomy(this);
        }

        /// <summary>
        /// Loads diplomatic relations from parsed country data.
        /// Supports Paradox-style keywords: give_guarantee, declare_war, create_alliance
        /// </summary>
        private void LoadDiplomaticRelations(ILookup<string, object> parsedData)
        {
            // Load guarantees: give_guarantee = ROK
            if (parsedData.Contains("give_guarantee"))
            {
                foreach (var target in parsedData["give_guarantee"])
                {
                    var targetTag = target?.ToString();
                    if (!string.IsNullOrEmpty(targetTag))
                    {
                        var guarantee = new GuaranteeRelation
                        {
                            FromCountryTag = this.Tag,
                            ToCountryTag = targetTag,
                        };
                        DiplomaticRelations.Add(guarantee);
                    }
                }
            }

            // Load wars: declare_war = { target = DRK }
            if (parsedData.Contains("declare_war"))
            {
                foreach (var warData in parsedData["declare_war"])
                {
                    if (
                        warData is ILookup<string, object> warLookup
                        && warLookup.Contains("target")
                    )
                    {
                        var targetTag = warLookup["target"].Single()?.ToString();
                        if (!string.IsNullOrEmpty(targetTag))
                        {
                            var war = new WarRelation
                            {
                                FromCountryTag = this.Tag,
                                ToCountryTag = targetTag,
                                Aggressor = this.Tag,
                            };
                            DiplomaticRelations.Add(war);
                        }
                    }
                }
            }

            // Load alliances: create_alliance = FRA
            if (parsedData.Contains("create_alliance"))
            {
                foreach (var target in parsedData["create_alliance"])
                {
                    var targetTag = target?.ToString();
                    if (!string.IsNullOrEmpty(targetTag))
                    {
                        var alliance = new AllianceRelation
                        {
                            FromCountryTag = this.Tag,
                            ToCountryTag = targetTag,
                        };
                        DiplomaticRelations.Add(alliance);
                    }
                }
            }
        }

        public override void OnTickDone(object sender, EventArgs e)
        {
            Economy.GrowProvinceIndustrialization();
            base.OnTickDone(sender, e);
        }

        /// <summary>
        /// Adds a guarantee to another country.
        /// </summary>
        public void AddGuarantee(string targetCountryTag, DateTime? startDate = null)
        {
            var guarantee = new GuaranteeRelation
            {
                FromCountryTag = this.Tag,
                ToCountryTag = targetCountryTag,
                StartDate = startDate,
            };
            DiplomaticRelations.Add(guarantee);
        }

        /// <summary>
        /// Removes a guarantee to a specific country.
        /// </summary>
        public void RemoveGuarantee(string targetCountryTag)
        {
            DiplomaticRelations.RemoveAll(r =>
                r is GuaranteeRelation && r.ToCountryTag == targetCountryTag
            );
        }

        /// <summary>
        /// Checks if this country is guaranteeing another country.
        /// </summary>
        public bool IsGuaranteeing(string countryTag, DateTime currentDate)
        {
            return DiplomaticRelations
                .OfType<GuaranteeRelation>()
                .Any(g => g.ToCountryTag == countryTag && g.IsActive(currentDate));
        }

        /// <summary>
        /// Gets all countries guaranteed by this country.
        /// </summary>
        public IEnumerable<string> GetGuaranteedCountries(DateTime currentDate)
        {
            return DiplomaticRelations
                .OfType<GuaranteeRelation>()
                .Where(g => g.IsActive(currentDate))
                .Select(g => g.ToCountryTag);
        }
    }
}
