using ColdWarGameLogic.GameLogic;
using ColdWarGameLogic.GameWorld;
using OpenGSGLibrary.GameDataManager;

namespace ColdWarPrototype.Views
{
    /// <summary>
    /// Manages the display of the active (player-controlled) country stats.
    /// </summary>
    public class ActiveCountryInfo
    {
        private readonly ColdWarPrototype2.MainWindow _mainWindow;
        private readonly MasterController _gameController;

        public ActiveCountryInfo(
            ColdWarPrototype2.MainWindow mainWindow,
            MasterController gameController
        )
        {
            _mainWindow = mainWindow;
            _gameController = gameController;

            // Wire up textbox event for investment changes
            _mainWindow.IndustryInvestmentTextBox.KeyDown += IndustryInvestmentTextBox_KeyDown;
        }

        /// <summary>
        /// Updates the active country display with current player country data.
        /// </summary>
        public void UpdateActiveCountry(WorldState worldState)
        {
            var activeCountryTag = _gameController.TickHandler.ActivePlayerCountryTag;

            if (string.IsNullOrEmpty(activeCountryTag))
            {
                // Observer mode
                _mainWindow.ActiveCountryName.Text = "Observer Mode";
                ClearStats();
                return;
            }

            var countries = worldState.GetCountryTable();
            if (countries == null || !countries.TryGetValue(activeCountryTag, out var country))
            {
                _mainWindow.ActiveCountryName.Text = "No Active Country";
                ClearStats();
                return;
            }

            if (country is not CwpCountry cwpCountry)
                return;

            // Update display
            _mainWindow.ActiveCountryName.Text = $"{cwpCountry.FullName} ({cwpCountry.Tag})";
            _mainWindow.ActiveCountryProduction.Text =
                $"Production: {cwpCountry.GetTotalProduction():N0}";
            _mainWindow.ActiveCountryMilitaryStrength.Text =
                $"Military Strength: {cwpCountry.MilitaryStrength:F1}";
            _mainWindow.ActiveCountrySoftPower.Text = $"Soft Power: {cwpCountry.SoftPower:F1}";
            _mainWindow.ActiveCountryUnrest.Text = $"Unrest: {cwpCountry.Unrest:F1}";
            _mainWindow.ActiveCountryCivilTech.Text = $"Civil Tech: {cwpCountry.CivilTech:F1}";
            _mainWindow.ActiveCountryMilitaryTech.Text =
                $"Military Tech: {cwpCountry.MilitaryTech:F1}";

            // Update investment textbox (only if not currently being edited)
            if (!_mainWindow.IndustryInvestmentTextBox.Focused)
            {
                _mainWindow.IndustryInvestmentTextBox.Text =
                    cwpCountry.IndustryInvestmentPercent.ToString("F0");
            }
        }

        private void ClearStats()
        {
            _mainWindow.ActiveCountryProduction.Text = "Production: -";
            _mainWindow.ActiveCountryMilitaryStrength.Text = "Military Strength: -";
            _mainWindow.ActiveCountrySoftPower.Text = "Soft Power: -";
            _mainWindow.ActiveCountryUnrest.Text = "Unrest: -";
            _mainWindow.ActiveCountryCivilTech.Text = "Civil Tech: -";
            _mainWindow.ActiveCountryMilitaryTech.Text = "Military Tech: -";
            _mainWindow.IndustryInvestmentTextBox.Text = "";
        }

        /// <summary>
        /// Handles Enter key in investment textbox to apply changes.
        /// </summary>
        private void IndustryInvestmentTextBox_KeyDown(
            object? sender,
            System.Windows.Forms.KeyEventArgs e
        )
        {
            if (e.KeyCode != System.Windows.Forms.Keys.Enter)
                return;

            var activeCountryTag = _gameController.TickHandler.ActivePlayerCountryTag;
            if (string.IsNullOrEmpty(activeCountryTag))
                return;

            var countries = _gameController.TickHandler.GetState().GetCountryTable();
            if (countries == null || !countries.TryGetValue(activeCountryTag, out var country))
                return;

            if (country is not CwpCountry cwpCountry)
                return;

            // Parse and validate input
            if (float.TryParse(_mainWindow.IndustryInvestmentTextBox.Text, out var value))
            {
                cwpCountry.IndustryInvestmentPercent = Math.Clamp(value, 0.0f, 100.0f);

                // Update display with clamped value
                _mainWindow.IndustryInvestmentTextBox.Text =
                    cwpCountry.IndustryInvestmentPercent.ToString("F0");
            }
            else
            {
                // Invalid input - revert to current value
                _mainWindow.IndustryInvestmentTextBox.Text =
                    cwpCountry.IndustryInvestmentPercent.ToString("F0");
            }
        }
    }
}
