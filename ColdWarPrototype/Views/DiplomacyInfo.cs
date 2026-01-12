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

            if (country.DiplomaticRelations.Count == 0)
            {
                // Show "No diplomatic relations" message
                var item = new ListViewItem("None");
                item.SubItems.Add("-");
                motherWindow_.DiplomacyListView.Items.Add(item);
                return;
            }

            var currentDate = gameController_.GetGameDateTime();

            // Group relations by type for better organization
            var activeRelations = country
                .DiplomaticRelations.Where(r => r.IsActive(currentDate))
                .OrderBy(r => r.RelationType)
                .ThenBy(r => r.ToCountryTag);

            foreach (var relation in activeRelations)
            {
                var relationType = GetRelationTypeDisplay(relation);
                var targetCountry = GetCountryDisplayName(relation.ToCountryTag);

                var item = new ListViewItem(relationType);
                item.SubItems.Add(targetCountry);
                item.Tag = relation; // Store relation for potential future use

                // Optional: Color-code by relation type
                switch (relation)
                {
                    case WarRelation:
                        item.ForeColor = System.Drawing.Color.Red;
                        break;
                    case AllianceRelation:
                        item.ForeColor = System.Drawing.Color.Green;
                        break;
                    case GuaranteeRelation:
                        item.ForeColor = System.Drawing.Color.Blue;
                        break;
                }

                motherWindow_.DiplomacyListView.Items.Add(item);
            }
        }

        /// <summary>
        /// Gets a display-friendly name for the relation type.
        /// </summary>
        private string GetRelationTypeDisplay(DiplomaticRelation relation)
        {
            return relation switch
            {
                WarRelation => "At War",
                AllianceRelation => "Alliance",
                GuaranteeRelation => "Guarantee",
                _ => relation.RelationType,
            };
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
