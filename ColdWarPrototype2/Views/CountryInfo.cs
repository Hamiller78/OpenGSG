using System;
using Simulation;
using WorldData;

namespace ColdWarPrototype2.Views
{
    public class CountryInfo
    {
        private readonly MainWindow motherWindow_;
        private readonly MasterController controller_;

        public string CurrentTag { get; private set; } = string.Empty;
        public string CurrentName { get; private set; } = string.Empty;
        public (byte R, byte G, byte B)? CurrentColor { get; private set; } = null;

        public CountryInfo(MainWindow motherWindow, MasterController controller)
        {
            motherWindow_ = motherWindow;
            controller_ = controller;
        }

        public void UpdateCurrentCountry(WorldState state, string countryTag)
        {
            CurrentTag = countryTag ?? string.Empty;
            CurrentName = string.Empty;
            CurrentColor = null;

            if (
                state?.GetCountryTable() is { } table
                && table.TryGetValue(countryTag, out var country)
            )
            {
                CurrentName = country.GetName();
                var c = country.GetColor();
                CurrentColor = (c.Item1, c.Item2, c.Item3);

                // Optionally update UI on motherWindow_ here if you have controls:
                // motherWindow_.Text = CurrentName;
            }
        }

        public void UpdateCurrentCountry(WorldState state)
        {
            if (!string.IsNullOrEmpty(CurrentTag))
                UpdateCurrentCountry(state, CurrentTag);
        }
    }
}
