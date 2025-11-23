using System;
using Simulation;
using WorldData;

namespace ColdWarPrototype2.Views
{
    public class CountryInfo
    {
        private readonly Form1 motherWindow_;
        private readonly MasterController controller_;

        public CountryInfo(Form1 motherWindow, MasterController controller)
        {
            motherWindow_ = motherWindow;
            controller_ = controller;
        }

        public void UpdateCurrentCountry(WorldState state, string countryTag)
        {
            // stub: in future update UI controls with country data
            if (state?.GetCountryTable() is { } table && table.TryGetValue(countryTag, out var c))
            {
                // example: var name = c.GetName();
            }
        }
    }
}
