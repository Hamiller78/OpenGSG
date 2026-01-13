using System;
using System.Collections.Generic;
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
            motherWindow_.DiplomacyListView.Columns.Add("Type", 100);
            motherWindow_.DiplomacyListView.Columns.Add("Country", 120);
            motherWindow_.DiplomacyListView.Columns.Add("Value", 40); // For opinion values
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
        /// Refreshes the ListView with diplomatic relations and opinions.
        /// </summary>
        private void RefreshDiplomacyListView(Country country)
        {
            motherWindow_.DiplomacyListView.Items.Clear();

            // DEBUG
            System.Diagnostics.Debug.WriteLine($"=== Refreshing diplomacy for {country.Tag} ===");
            System.Diagnostics.Debug.WriteLine($"Relations: {country.DiplomaticRelations.Count}");
            System.Diagnostics.Debug.WriteLine($"Opinions: {country.Opinions.Count}");

            var currentDate = gameController_.GetGameDateTime();
            var allCountries = gameController_.TickHandler.GetState()?.GetCountryTable();

            if (allCountries == null)
                return;

            // === Section 1: Diplomatic Relations ===
            var relations = new List<(string direction, DiplomaticRelation relation)>();

            // Outgoing relations
            foreach (
                var relation in country.DiplomaticRelations.Where(r => r.IsActive(currentDate))
            )
            {
                relations.Add(("→", relation));
            }

            // Incoming relations
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

            // Sort and display relations
            if (relations.Count > 0)
            {
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
                    item.SubItems.Add(""); // No value for relations
                    item.Tag = relation;

                    // Color-code by relation type
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

            // === Section 2: Opinions ===
            var opinions = new List<(string direction, string targetTag, int value)>();

            // Outgoing opinions (this country's opinion of others)
            foreach (var opinion in country.Opinions)
            {
                var value = opinion.Value.GetTotalOpinion(currentDate);
                if (value != 0) // Only show non-neutral opinions
                {
                    opinions.Add(("→", opinion.Key, value));
                }
            }

            // Incoming opinions (others' opinion of this country)
            foreach (var otherCountry in allCountries.Values)
            {
                if (otherCountry.Tag == country.Tag)
                    continue;

                var opinionOfUs = otherCountry.GetOpinionOf(country.Tag, currentDate);
                if (opinionOfUs != 0)
                {
                    opinions.Add(("←", otherCountry.Tag, opinionOfUs));
                }
            }

            if (opinions.Count > 0)
            {
                // Add separator if we had relations
                if (relations.Count > 0)
                {
                    var separator = new ListViewItem("───────────");
                    separator.SubItems.Add("");
                    separator.SubItems.Add("");
                    motherWindow_.DiplomacyListView.Items.Add(separator);
                }

                // Sort opinions: outgoing first, then by value (most negative first)
                var sortedOpinions = opinions
                    .OrderBy(o => o.direction == "←" ? 1 : 0)
                    .ThenBy(o => o.value);

                foreach (var (direction, targetTag, value) in sortedOpinions)
                {
                    var opinionType = direction == "→" ? "Opinion of" : "Opinion from";
                    var targetCountry = GetCountryDisplayName(targetTag);
                    var valueStr = value > 0 ? $"+{value}" : value.ToString();

                    var item = new ListViewItem(opinionType);
                    item.SubItems.Add(targetCountry);
                    item.SubItems.Add(valueStr);

                    // Color-code by opinion value
                    item.ForeColor = GetOpinionColor(value);

                    motherWindow_.DiplomacyListView.Items.Add(item);
                }
            }

            // Show "None" if no relations or opinions
            if (relations.Count == 0 && opinions.Count == 0)
            {
                var item = new ListViewItem("None");
                item.SubItems.Add("-");
                item.SubItems.Add("");
                motherWindow_.DiplomacyListView.Items.Add(item);
            }
        }

        /// <summary>
        /// Gets color based on opinion value.
        /// </summary>
        private System.Drawing.Color GetOpinionColor(int value)
        {
            return value switch
            {
                >= 75 => System.Drawing.Color.DarkGreen, // Very friendly
                >= 50 => System.Drawing.Color.Green, // Friendly
                >= 25 => System.Drawing.Color.LightGreen, // Positive
                > 0 => System.Drawing.Color.Gray, // Slightly positive
                0 => System.Drawing.Color.Black, // Neutral
                > -25 => System.Drawing.Color.Gray, // Slightly negative
                > -50 => System.Drawing.Color.Orange, // Negative
                > -75 => System.Drawing.Color.OrangeRed, // Hostile
                _ => System
                    .Drawing
                    .Color
                    .Red // Very hostile
                ,
            };
        }

        /// <summary>
        /// Determines if an incoming relation should be shown.
        /// </summary>
        private bool ShouldShowIncoming(DiplomaticRelation relation, string currentCountryTag)
        {
            return relation switch
            {
                WarRelation => true,
                AllianceRelation => true,
                GuaranteeRelation => true,
                _ => true,
            };
        }

        /// <summary>
        /// Gets the "other" country in a relation.
        /// </summary>
        private string GetOtherCountryTag(DiplomaticRelation relation, string currentCountryTag)
        {
            return relation.FromCountryTag == currentCountryTag
                ? relation.ToCountryTag
                : relation.FromCountryTag;
        }

        /// <summary>
        /// Gets a display-friendly name for the relation type.
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
        /// Gets the display name for a country.
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
