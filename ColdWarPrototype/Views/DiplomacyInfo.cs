using System;
using System.Linq;
using System.Windows.Forms;
using ColdWarGameLogic.Diplomacy;
using ColdWarGameLogic.GameLogic;
using ColdWarGameLogic.GameWorld;
using ColdWarPrototype.Controller;
using ColdWarPrototype2;
using OpenGSGLibrary.Diplomacy;
using OpenGSGLibrary.GameDataManager;

namespace ColdWarPrototype.Views
{
    /// <summary>
    /// Adapter between diplomatic relations data and the DiplomacyListView control.
    /// </summary>
    public class DiplomacyInfo
    {
        private readonly MainWindow motherWindow_;
        private readonly MasterController gameController_;
        private string currentCountryTag_ = string.Empty;

        public DiplomacyInfo(MainWindow motherWindow, MasterController controller)
        {
            motherWindow_ = motherWindow ?? throw new ArgumentNullException(nameof(motherWindow));
            gameController_ = controller ?? throw new ArgumentNullException(nameof(controller));

            SetupListView();
        }

        /// <summary>
        /// Configures the ListView columns.
        /// </summary>
        private void SetupListView()
        {
            motherWindow_.DiplomacyListView.View = View.Details;
            motherWindow_.DiplomacyListView.FullRowSelect = true;
            motherWindow_.DiplomacyListView.GridLines = true;

            // Add columns
            motherWindow_.DiplomacyListView.Columns.Add("Type", 80);
            motherWindow_.DiplomacyListView.Columns.Add("Country", 140);
        }

        /// <summary>
        /// Handles country selection change from the mouse controller.
        /// </summary>
        public void HandleCountryChanged(object? sender, CountryEventArgs e)
        {
            UpdateDiplomacy(gameController_.TickHandler.GetState(), e.CountryTag);
        }

        /// <summary>
        /// Updates the diplomacy display for the current country.
        /// </summary>
        public void UpdateCurrentCountry(WorldState state)
        {
            if (!string.IsNullOrEmpty(currentCountryTag_))
                UpdateDiplomacy(state, currentCountryTag_);
        }

        /// <summary>
        /// Updates the diplomacy ListView for the specified country.
        /// </summary>
        public void UpdateDiplomacy(WorldState state, string countryTag)
        {
            if (state == null || string.IsNullOrEmpty(countryTag))
            {
                ClearDiplomacyDisplay();
                return;
            }

            var countries = state.GetCountryTable();
            if (countries == null || !countries.TryGetValue(countryTag, out var country))
            {
                ClearDiplomacyDisplay();
                return;
            }

            currentCountryTag_ = countryTag;

            // Thread-safe UI update
            if (motherWindow_.InvokeRequired)
            {
                motherWindow_.BeginInvoke(() => RefreshDiplomacyListView(country));
            }
            else
            {
                RefreshDiplomacyListView(country);
            }
        }

        /// <summary>
        /// Refreshes the ListView with diplomatic relations.
        /// </summary>
        private void RefreshDiplomacyListView(Country country)
        {
            motherWindow_.DiplomacyListView.Items.Clear();

            var currentDate = gameController_.GetGameDateTime();
            var allCountries = gameController_.TickHandler.GetState()?.GetCountryTable();

            if (allCountries == null)
                return;

            var relations = new List<(string direction, DiplomaticRelation relation)>();

            // 1. Outgoing relations (this country -> others)
            foreach (
                var relation in country.DiplomaticRelations.Where(r => r.IsActive(currentDate))
            )
            {
                relations.Add(("→", relation));
            }

            // 2. Incoming relations (others -> this country)
            foreach (var otherCountry in allCountries.Values)
            {
                if (otherCountry.Tag == country.Tag)
                    continue;

                foreach (
                    var relation in otherCountry.DiplomaticRelations.Where(r =>
                        r.IsActive(currentDate)
                    )
                )
                {
                    if (relation.ToCountryTag == country.Tag)
                    {
                        if (ShouldShowIncoming(relation, country.Tag))
                        {
                            relations.Add(("←", relation));
                        }
                    }
                }
            }

            if (relations.Count == 0)
            {
                var item = new ListViewItem("None");
                item.SubItems.Add("-");
                motherWindow_.DiplomacyListView.Items.Add(item);
                return;
            }

            // Sort: outgoing first, then by type, then by country
            var sortedRelations = relations
                .OrderBy(r => r.direction == "←" ? 1 : 0)
                .ThenBy(r => r.relation.RelationType)
                .ThenBy(r => GetOtherCountryTag(r.relation, country.Tag));

            foreach (var (direction, relation) in sortedRelations)
            {
                var relationType = GetRelationTypeDisplay(relation, direction);
                var otherCountryTag = GetOtherCountryTag(relation, country.Tag);
                var targetCountry = GetCountryDisplayName(otherCountryTag);

                var item = new ListViewItem(relationType);
                item.SubItems.Add(targetCountry);
                item.Tag = relation;

                switch (relation)
                {
                    case WarRelation:
                        item.ForeColor = System.Drawing.Color.Red;
                        break;
                    case AllianceRelation:
                        item.ForeColor = System.Drawing.Color.Green;
                        break;
                    case GuaranteeRelation:
                        item.ForeColor =
                            direction == "→"
                                ? System.Drawing.Color.Blue
                                : System.Drawing.Color.Cyan;
                        break;
                }

                motherWindow_.DiplomacyListView.Items.Add(item);
            }
        }

        /// <summary>
        /// Determines if an incoming relation should be shown (avoids duplicates for bidirectional relations).
        /// </summary>
        private bool ShouldShowIncoming(DiplomaticRelation relation, string currentCountryTag)
        {
            return relation switch
            {
                // Wars and alliances are typically bidirectional, but show incoming anyway
                // (they should exist in both directions, but we show them from perspective)
                WarRelation => true,
                AllianceRelation => true,

                // Guarantees are unidirectional - show incoming
                GuaranteeRelation => true,

                _ => true,
            };
        }

        /// <summary>
        /// Gets the "other" country in a relation (not the current country).
        /// </summary>
        private string GetOtherCountryTag(DiplomaticRelation relation, string currentCountryTag)
        {
            return relation.FromCountryTag == currentCountryTag
                ? relation.ToCountryTag
                : relation.FromCountryTag;
        }

        /// <summary>
        /// Gets a display-friendly name for the relation type, with direction indicator.
        /// </summary>
        private string GetRelationTypeDisplay(DiplomaticRelation relation, string direction)
        {
            var baseType = relation switch
            {
                WarRelation => "At War",
                AllianceRelation => "Alliance",
                GuaranteeRelation => direction == "→" ? "Guaranteeing" : "Guaranteed by",
                _ => relation.RelationType,
            };

            return baseType;
        }

        /// <summary>
        /// Gets the display name for a country (full name if available, otherwise tag).
        /// </summary>
        private string GetCountryDisplayName(string countryTag)
        {
            var countries = gameController_.TickHandler.GetState()?.GetCountryTable();
            if (countries != null && countries.TryGetValue(countryTag, out var country))
            {
                if (country is CwpCountry cwp && !string.IsNullOrEmpty(cwp.FullName))
                {
                    return cwp.FullName;
                }
                return country.Name;
            }
            return countryTag;
        }

        /// <summary>
        /// Clears the diplomacy display.
        /// </summary>
        private void ClearDiplomacyDisplay()
        {
            currentCountryTag_ = string.Empty;
            if (motherWindow_.InvokeRequired)
            {
                motherWindow_.BeginInvoke(() => motherWindow_.DiplomacyListView.Items.Clear());
            }
            else
            {
                motherWindow_.DiplomacyListView.Items.Clear();
            }
        }
    }
}
