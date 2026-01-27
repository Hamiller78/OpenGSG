using ColdWarPrototype2;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.Military;

namespace ColdWarPrototype.Views
{
    /// <summary>
    /// View helper for managing military units display and movement.
    /// </summary>
    public class MilitaryUnitsView
    {
        private readonly MainWindow _motherWindow;
        private MilitaryFormation? _selectedFormation;

        public MilitaryUnitsView(MainWindow motherWindow)
        {
            _motherWindow = motherWindow;
        }

        /// <summary>
        /// Updates the military units list for the active country.
        /// </summary>
        public void UpdateMilitaryList(WorldState worldState, string? activeCountryTag)
        {
            var listView = _motherWindow.MilitaryUnitsListView;
            listView.Items.Clear();

            if (string.IsNullOrEmpty(activeCountryTag))
                return;

            var countries = worldState.GetCountryTable();
            if (countries == null || !countries.TryGetValue(activeCountryTag, out var country))
                return;

            if (country.Military == null)
                return;

            var provinceTable = worldState.GetProvinceTable();

            // Add all armies
            foreach (var formation in country.Military.Armies)
            {
                AddFormationToList(formation, provinceTable);
            }

            // Add all air forces
            foreach (var formation in country.Military.AirForces)
            {
                AddFormationToList(formation, provinceTable);
            }
        }

        private void AddFormationToList(
            MilitaryFormation formation,
            IDictionary<int, Province>? provinceTable
        )
        {
            var item = new ListViewItem(formation.Branch); // Type column

            // Location column
            string locationName = formation.Location.ToString();
            if (
                provinceTable != null
                && provinceTable.TryGetValue(formation.Location, out var province)
            )
            {
                locationName = $"{province.Name} ({formation.Location})";
            }
            item.SubItems.Add(locationName);

            // Units column
            var unitsSummary = string.Join(
                ", ",
                formation.Units.Select(kvp => $"{kvp.Value}x {kvp.Key}")
            );
            item.SubItems.Add(unitsSummary);

            // Readiness column
            item.SubItems.Add(formation.Readiness);

            // Strength column - TODO: Add when unit definitions are more accessible
            int totalUnits = formation.Units.Values.Sum();
            item.SubItems.Add($"{totalUnits} units");

            // Store formation reference in Tag
            item.Tag = formation;

            _motherWindow.MilitaryUnitsListView.Items.Add(item);
        }

        /// <summary>
        /// Updates the Move button enabled state based on selection and pinned province.
        /// </summary>
        public void UpdateMoveButtonState(bool isProvinceSelected, bool isFormationSelected)
        {
            _motherWindow.MoveUnitsButton.Enabled = isProvinceSelected && isFormationSelected;
        }

        /// <summary>
        /// Gets the currently selected formation.
        /// </summary>
        public MilitaryFormation? GetSelectedFormation()
        {
            if (_motherWindow.MilitaryUnitsListView.SelectedItems.Count == 0)
                return null;

            return _motherWindow.MilitaryUnitsListView.SelectedItems[0].Tag as MilitaryFormation;
        }
    }
}
