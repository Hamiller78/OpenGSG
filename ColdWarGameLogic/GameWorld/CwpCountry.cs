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

        // Country Stats (0-100 normalized values)
        /// <summary>
        /// Military strength indicator (0-100).
        /// Calculated from army size, equipment, and economy.
        /// </summary>
        public float MilitaryStrength { get; set; } = 0.0f;

        /// <summary>
        /// Soft power indicator (0-100).
        /// Represents cultural influence, diplomacy effectiveness, and international standing.
        /// </summary>
        public float SoftPower { get; set; } = 50.0f;

        /// <summary>
        /// Internal unrest level (0-100).
        /// Higher values indicate greater instability, risk of coups, or civil unrest.
        /// </summary>
        public float Unrest { get; set; } = 0.0f;

        /// <summary>
        /// Civil technology level (0-100).
        /// Represents civilian industrial and scientific advancement.
        /// Affects production efficiency.
        /// </summary>
        public float CivilTech { get; set; } = 50.0f;

        /// <summary>
        /// Military technology level (0-100).
        /// Represents military equipment, doctrine, and weapons advancement.
        /// Affects army effectiveness.
        /// </summary>
        public float MilitaryTech { get; set; } = 50.0f;

        /// <summary>
        /// Percentage of post-military budget reinvested into industry (0-100).
        /// Remainder goes to standard of living.
        /// Player/AI controlled parameter.
        /// </summary>
        public float IndustryInvestmentPercent { get; set; } = 50.0f; // Default: balanced

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

            // Load country stats
            if (parsedData.Contains("soft_power"))
            {
                var value = parsedData["soft_power"].Single()?.ToString();
                if (float.TryParse(value, out var softPower))
                {
                    SoftPower = Math.Clamp(softPower, 0.0f, 100.0f);
                }
            }

            if (parsedData.Contains("unrest"))
            {
                var value = parsedData["unrest"].Single()?.ToString();
                if (float.TryParse(value, out var unrest))
                {
                    Unrest = Math.Clamp(unrest, 0.0f, 100.0f);
                }
            }

            // Tech levels
            if (parsedData.Contains("civil_tech"))
            {
                var value = parsedData["civil_tech"].Single()?.ToString();
                if (float.TryParse(value, out var civilTech))
                {
                    CivilTech = Math.Clamp(civilTech, 0.0f, 100.0f);
                }
            }

            if (parsedData.Contains("military_tech"))
            {
                var value = parsedData["military_tech"].Single()?.ToString();
                if (float.TryParse(value, out var militaryTech))
                {
                    MilitaryTech = Math.Clamp(militaryTech, 0.0f, 100.0f);
                }
            }

            // MilitaryStrength is calculated, not loaded directly
            // (though we could allow overriding it in history files if needed)
            if (parsedData.Contains("military_strength"))
            {
                var value = parsedData["military_strength"].Single()?.ToString();
                if (float.TryParse(value, out var milStrength))
                {
                    MilitaryStrength = Math.Clamp(milStrength, 0.0f, 100.0f);
                }
            }

            // Load economic policy
            if (parsedData.Contains("industry_investment"))
            {
                var value = parsedData["industry_investment"].Single()?.ToString();
                if (float.TryParse(value, out var investment))
                {
                    IndustryInvestmentPercent = Math.Clamp(investment, 0.0f, 100.0f);
                }
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

        /// <summary>
        /// Gets the total industrial production from all owned provinces.
        /// Applies civil tech multiplier to raw production.
        /// </summary>
        public long GetTotalProduction()
        {
            long rawProduction = 0;
            foreach (var province in Provinces)
            {
                if (province is CwpProvince cwpProv)
                {
                    rawProduction += cwpProv.Production;
                }
            }

            // Apply civil tech multiplier
            float techMultiplier = 1.0f + (CivilTech / 200.0f); // Up to 1.5x at 100 tech
            return (long)(rawProduction * techMultiplier);
        }

        /// <summary>
        /// Calculates monthly military upkeep cost.
        /// TODO: Based on number/type of armies when army system is implemented.
        /// </summary>
        private long CalculateMilitaryUpkeep()
        {
            // Placeholder: For now, use a percentage of production
            // Later: Sum upkeep costs of all army units
            long totalProduction = GetTotalProduction();
            return (long)(totalProduction * 0.15); // 15% of production for now
        }

        /// <summary>
        /// Recalculates country statistics based on current state.
        /// Called monthly during game tick.
        /// </summary>
        public override void RecalculateStats()
        {
            long totalProduction = GetTotalProduction();
            long militaryUpkeep = CalculateMilitaryUpkeep();
            long remainingBudget = Math.Max(0, totalProduction - militaryUpkeep);

            // Split remaining budget
            float investPercent = Math.Clamp(IndustryInvestmentPercent, 0.0f, 100.0f);
            long industryInvestment = (long)(remainingBudget * investPercent / 100.0);
            long standardOfLivingSpending = remainingBudget - industryInvestment;

            // Update province industrialization based on investment
            UpdateProvinceIndustrialization(industryInvestment, totalProduction);

            // Update Civil Tech based on standard of living spending
            UpdateCivilTech(standardOfLivingSpending, totalProduction);

            // Update Military Tech based on military spending
            UpdateMilitaryTech(militaryUpkeep, totalProduction);

            // Update Unrest based on standard of living
            UpdateUnrest(standardOfLivingSpending, totalProduction);

            // Update MilitaryStrength based on production, military tech, and armies
            UpdateMilitaryStrength(totalProduction, militaryUpkeep);

            // Update SoftPower based on various factors
            UpdateSoftPower(standardOfLivingSpending, totalProduction);

            // Ensure all values stay in valid range
            MilitaryStrength = Math.Clamp(MilitaryStrength, 0.0f, 100.0f);
            SoftPower = Math.Clamp(SoftPower, 0.0f, 100.0f);
            Unrest = Math.Clamp(Unrest, 0.0f, 100.0f);
            CivilTech = Math.Clamp(CivilTech, 0.0f, 100.0f);
            MilitaryTech = Math.Clamp(MilitaryTech, 0.0f, 100.0f);
        }

        /// <summary>
        /// Updates industrialization in all provinces based on investment.
        /// Investment maintains/grows industrialization, lack of it causes decay.
        /// </summary>
        private void UpdateProvinceIndustrialization(long industryInvestment, long totalProduction)
        {
            if (Provinces.Count == 0)
                return;

            // Calculate investment per production unit (how much we're investing relative to output)
            float investmentRatio =
                totalProduction > 0 ? (float)industryInvestment / totalProduction : 0;

            // Natural equilibrium drift point (without investment, industrialization drifts toward this)
            const float naturalEquilibrium = 30.0f; // Subsistence economy level

            foreach (var province in Provinces)
            {
                if (province is not CwpProvince cwpProv)
                    continue;

                float currentIndustrialization = cwpProv.Industrialization;

                // Calculate drift toward equilibrium
                // Higher investment = higher equilibrium point
                // 0% investment → drift to 30 (natural decay)
                // 25% investment → maintain current level (roughly)
                // 50%+ investment → grow toward 100
                float targetEquilibrium = naturalEquilibrium + (investmentRatio * 140.0f);
                targetEquilibrium = Math.Clamp(targetEquilibrium, naturalEquilibrium, 100.0f);

                // Drift toward target equilibrium
                float driftRate = 0.05f; // 5% per month
                float industrializationChange =
                    (targetEquilibrium - currentIndustrialization) * driftRate;

                // Apply change
                cwpProv.Industrialization = Math.Clamp(
                    currentIndustrialization + industrializationChange,
                    0.0f,
                    100.0f
                );
            }
        }

        /// <summary>
        /// Updates civil technology based on standard of living spending.
        /// Higher living standards = more education, research, innovation.
        /// </summary>
        private void UpdateCivilTech(long livingSpending, long totalProduction)
        {
            if (totalProduction == 0)
                return;

            // Living standard ratio: 0.0 to 1.0
            float livingRatio = (float)livingSpending / totalProduction;

            // Civil tech growth rate: 0-0.5 points per month depending on spending
            // High SoL investment = research, universities, innovation
            float techGrowth = livingRatio * 0.8f; // Max 0.4 points/month at 50% SoL spending

            // Education multiplier: provinces with higher average education boost tech growth
            float avgEducation = GetAverageEducation();
            float educationBonus = 1.0f + (avgEducation / 200.0f); // Up to 1.5x at 100 education
            techGrowth *= educationBonus;

            // Diminishing returns as tech approaches 100
            float diminishingFactor = (100.0f - CivilTech) / 100.0f;
            techGrowth *= diminishingFactor;

            CivilTech += techGrowth;
        }

        /// <summary>
        /// Updates military technology based on military spending.
        /// Higher militarization = better equipment, doctrines, weapons.
        /// </summary>
        private void UpdateMilitaryTech(long militarySpending, long totalProduction)
        {
            if (totalProduction == 0)
                return;

            // Military spending ratio: 0.0 to 1.0
            float militaryRatio = (float)militarySpending / totalProduction;

            // Military tech growth rate based on military spending
            // Higher spending = research into weapons, equipment, tactics
            float techGrowth = militaryRatio * 1.5f; // Faster than civil tech

            // Wars accelerate military tech (necessity)
            if (IsAtWar(DateTime.Now))
            {
                techGrowth *= 1.5f; // +50% during wartime
            }

            // Diminishing returns
            float diminishingFactor = (100.0f - MilitaryTech) / 100.0f;
            techGrowth *= diminishingFactor;

            MilitaryTech += techGrowth;
        }

        /// <summary>
        /// Gets average education level across all provinces.
        /// </summary>
        private float GetAverageEducation()
        {
            if (Provinces.Count == 0)
                return 0;

            float totalEducation = 0;
            int count = 0;

            foreach (var province in Provinces)
            {
                if (province is CwpProvince cwpProv)
                {
                    totalEducation += cwpProv.Education;
                    count++;
                }
            }

            return count > 0 ? totalEducation / count : 0;
        }

        /// <summary>
        /// Updates unrest based on standard of living spending.
        /// Low spending = rising unrest, high spending = falling unrest.
        /// </summary>
        private void UpdateUnrest(long livingSpending, long totalProduction)
        {
            if (totalProduction == 0)
                return;

            // Living standard ratio: 0.0 to 1.0
            float livingRatio = (float)livingSpending / totalProduction;

            // Target unrest based on living standards
            // 0% spending -> 80 unrest
            // 25% spending -> 20 unrest (good balance)
            // 50%+ spending -> 0 unrest
            float targetUnrest = Math.Max(0, 80.0f - (livingRatio * 160.0f));

            // Gradually move toward target (10% adjustment per month)
            float unrestChange = (targetUnrest - Unrest) * 0.1f;
            Unrest += unrestChange;

            // Wars increase unrest
            if (IsAtWar(DateTime.Now))
            {
                Unrest += 2.0f; // +2 per month during war
            }
        }

        /// <summary>
        /// Updates military strength based on production capacity, upkeep, and tech.
        /// </summary>
        private void UpdateMilitaryStrength(long totalProduction, long militaryUpkeep)
        {
            // Military strength correlates with production base, spending, and military tech

            if (totalProduction == 0)
            {
                MilitaryStrength = 0;
                return;
            }

            // Base strength from production capacity (0-40 points)
            float productionFactor = Math.Min(40.0f, totalProduction / 1000.0f);

            // Additional strength from military spending (0-30 points)
            float spendingRatio = (float)militaryUpkeep / totalProduction;
            float spendingFactor = Math.Min(30.0f, spendingRatio * 150.0f);

            // Military tech contribution (0-30 points)
            float techFactor = MilitaryTech * 0.3f;

            // TODO: Add army units count/quality when army system exists
            float targetStrength = productionFactor + spendingFactor + techFactor;

            // Gradually adjust toward calculated strength (20% per month)
            MilitaryStrength += (targetStrength - MilitaryStrength) * 0.2f;
        }

        /// <summary>
        /// Updates soft power based on living standards, alliances, stability, and tech.
        /// </summary>
        private void UpdateSoftPower(long livingSpending, long totalProduction)
        {
            // Soft power is influenced by:
            // 1. High living standards (shows successful system)
            // 2. Low unrest (stability attracts)
            // 3. Alliances (diplomatic connections)
            // 4. Civil tech level (innovation prestige)

            float targetSoftPower = 0;

            // Living standards contribution (0-30 points)
            if (totalProduction > 0)
            {
                float livingRatio = (float)livingSpending / totalProduction;
                targetSoftPower += Math.Min(30.0f, livingRatio * 60.0f);
            }

            // Stability contribution (0-30 points)
            targetSoftPower += (100.0f - Unrest) * 0.3f;

            // Civil tech contribution (0-25 points)
            targetSoftPower += CivilTech * 0.25f;

            // Alliance contribution (0-15 points)
            var currentDate = DateTime.Now; // TODO: Get proper game date
            int allianceCount = DiplomaticRelations
                .OfType<AllianceRelation>()
                .Count(a => a.IsActive(currentDate));
            targetSoftPower += Math.Min(15.0f, allianceCount * 5.0f);

            // Gradually adjust (slower than other stats - reputation changes slowly)
            SoftPower += (targetSoftPower - SoftPower) * 0.05f; // 5% adjustment/month
        }
    }
}
