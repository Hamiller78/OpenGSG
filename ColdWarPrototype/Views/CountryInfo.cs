using ColdWarGameLogic.GameLogic;
using ColdWarGameLogic.GameWorld;
using ColdWarPrototype.Controller;
using ColdWarPrototype2;
using OpenGSGLibrary.GameDataManager;

namespace ColdWarPrototype.Views
{
    public class CountryInfo
    {
        private readonly MainWindow motherWindow_;
        private readonly MasterController gameController_;

        public string CurrentTag { get; private set; } = string.Empty;
        public string CurrentName { get; private set; } = string.Empty;
        public (byte R, byte G, byte B)? CurrentColor { get; private set; } = null;

        public CountryInfo(MainWindow motherWindow, MasterController controller)
        {
            motherWindow_ = motherWindow;
            gameController_ = controller;
        }

        public void HandleCountryChanged(object? sender, CountryEventArgs e)
        {
            UpdateCountryInfo(gameController_.tickHandler.GetState(), e.CountryTag);
        }

        public void UpdateCurrentCountry(WorldState state)
        {
            if (!string.IsNullOrEmpty(CurrentTag))
                UpdateCountryInfo(state, CurrentTag);
        }

        public void UpdateCountryInfo(WorldState state, string countryTag)
        {
            CurrentTag = countryTag ?? string.Empty;
            var currentCountry = (CwpCountry)state.GetCountryTable()[countryTag];
            motherWindow_.CountryName.Text = currentCountry.longName;
            motherWindow_.CountryLeader.Text = currentCountry.leader;
            motherWindow_.CountryGovernment.Text = currentCountry.government;
            motherWindow_.CountryAllegiance.Text = currentCountry.allegiance;
            // CountryProduction.Text = tickHandler_.GetState().GetCountryProduction(currentCountryTag_)
            motherWindow_.FlagPictureBox.Image = currentCountry.Flag;

            //CurrentName = string.Empty;
            //CurrentColor = null;

            //if (
            //    state?.GetCountryTable() is { } table
            //    && table.TryGetValue(countryTag, out var country)
            //)
            //{
            //    CurrentName = country.GetName();
            //    var c = country.GetColor();
            //    CurrentColor = (c.Item1, c.Item2, c.Item3);
            //}
        }
    }
}
