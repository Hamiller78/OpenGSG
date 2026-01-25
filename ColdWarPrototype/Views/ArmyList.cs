using ColdWarPrototype2;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.Military;

namespace ColdWarPrototype.Views
{
    /// <summary>
    /// Simple view helper for showing military formations in a province.
    /// Provides methods to get formations for a province and to fill a ListBox control.
    /// </summary>
    public class ArmyList
    {
        private readonly MainWindow _motherWindow;

        private int _currentProvinceId;
        private bool _isChoosingTarget = false;
        private List<MilitaryFormation> _formationsInProvince = new List<MilitaryFormation>();
        private List<MilitaryFormation> _selectedFormations = new List<MilitaryFormation>();

        public ArmyList(MainWindow motherWindow)
        {
            _motherWindow = motherWindow;
        }

        /// <summary>
        /// Returns the list of military formations present in the given province.
        /// Searches all countries' militaries for formations stationed at this location.
        /// </summary>
        public List<MilitaryFormation> GetFormationsInProvince(WorldState state, int provinceId)
        {
            var formations = new List<MilitaryFormation>();

            if (state == null)
                return formations;

            var countries = state.GetCountryTable();
            if (countries == null)
                return formations;

            foreach (var country in countries.Values)
            {
                if (country.Military == null)
                    continue;

                // Add armies in this province
                formations.AddRange(country.Military.Armies.Where(a => a.Location == provinceId));

                // Add air forces in this province
                formations.AddRange(
                    country.Military.AirForces.Where(a => a.Location == provinceId)
                );
            }

            return formations;
        }

        /// <summary>
        /// Fills the given ListBox control with the military formations in the specified province.
        /// Shows formation type, owning country, and unit composition.
        /// </summary>
        public void FillListBox(ListBox listBox, WorldState state, int provinceId)
        {
            listBox.Items.Clear();
            var formations = GetFormationsInProvince(state, provinceId);

            foreach (var formation in formations)
            {
                // Find owning country
                var ownerTag = FindOwningCountry(state, formation);

                // Build display string
                var unitSummary = string.Join(
                    ", ",
                    formation.Units.Select(kvp => $"{kvp.Value}x {kvp.Key}")
                );

                var displayText = $"[{ownerTag}] {formation.Branch}: {unitSummary}";

                listBox.Items.Add(displayText);
            }
        }

        /// <summary>
        /// Updates the mother window's ArmyListBox with formations from the given province.
        /// Stores the formations internally for later selection/orders.
        /// </summary>
        public void UpdateArmyListBox(WorldState currentState, int mouseProvinceId)
        {
            _currentProvinceId = mouseProvinceId;
            _formationsInProvince = GetFormationsInProvince(currentState, mouseProvinceId);

            if (_motherWindow?.ArmyListBox == null)
                return;

            var lb = _motherWindow.ArmyListBox;
            lb.BeginUpdate();
            lb.Items.Clear();

            foreach (var formation in _formationsInProvince)
            {
                // Find owning country
                var ownerTag = FindOwningCountry(currentState, formation);

                // Build display string with readiness indicator
                var unitSummary = string.Join(
                    ", ",
                    formation.Units.Select(kvp => $"{kvp.Value}x {kvp.Key}")
                );

                var readinessIcon = GetReadinessIcon(formation.Readiness);
                var displayText = $"{readinessIcon} [{ownerTag}] {formation.Branch}: {unitSummary}";

                lb.Items.Add(displayText);
            }

            lb.EndUpdate();
        }

        /// <summary>
        /// Finds which country owns a specific formation.
        /// </summary>
        private string FindOwningCountry(WorldState state, MilitaryFormation formation)
        {
            var countries = state?.GetCountryTable();
            if (countries == null)
                return "???";

            foreach (var country in countries.Values)
            {
                if (country.Military == null)
                    continue;

                if (
                    country.Military.Armies.Contains(formation)
                    || country.Military.AirForces.Contains(formation)
                )
                {
                    return country.Tag;
                }
            }

            return "???";
        }

        /// <summary>
        /// Gets a visual indicator for readiness state.
        /// </summary>
        private string GetReadinessIcon(string readiness)
        {
            return readiness?.ToLower() switch
            {
                "active" => "⚔️",
                "ready" => "✓",
                "training" => "🎓",
                "reserve" => "💤",
                _ => "•",
            };
        }

        /// <summary>
        /// Gets the formation at the specified index in the current province.
        /// Used when user selects a formation from the list.
        /// </summary>
        public MilitaryFormation? GetFormationAtIndex(int index)
        {
            if (index < 0 || index >= _formationsInProvince.Count)
                return null;

            return _formationsInProvince[index];
        }

        /// <summary>
        /// Gets all formations currently displayed in the list.
        /// </summary>
        public List<MilitaryFormation> GetCurrentFormations()
        {
            return _formationsInProvince;
        }

        /// <summary>
        /// Sets whether we're in "choose target province" mode for march orders.
        /// </summary>
        public void SetChoosingTargetMode(bool isChoosing)
        {
            _isChoosingTarget = isChoosing;
        }

        /// <summary>
        /// Gets whether we're in target selection mode.
        /// </summary>
        public bool IsChoosingTarget => _isChoosingTarget;

        /// <summary>
        /// Adds or removes a formation from the selection.
        /// </summary>
        public void ToggleFormationSelection(int index)
        {
            var formation = GetFormationAtIndex(index);
            if (formation == null)
                return;

            if (_selectedFormations.Contains(formation))
                _selectedFormations.Remove(formation);
            else
                _selectedFormations.Add(formation);
        }

        /// <summary>
        /// Gets currently selected formations.
        /// </summary>
        public List<MilitaryFormation> GetSelectedFormations()
        {
            return _selectedFormations;
        }

        /// <summary>
        /// Clears the selection.
        /// </summary>
        public void ClearSelection()
        {
            _selectedFormations.Clear();
        }
    }
}
