using System;
using System.Reflection;
using Gui;
using Simulation;
using WorldData;

namespace ColdWarPrototype2.Views
{
    public class ProvinceInfo
    {
        private readonly MainWindow motherWindow_;
        private readonly MasterController controller_;
        private int currentProvinceId_ = -1;

        // Exposed data fields (can be bound to UI by caller)
        public string Name { get; private set; } = string.Empty;
        public long Population { get; private set; }
        public long Industrialization { get; private set; }
        public long Education { get; private set; }
        public long Production { get; private set; }
        public string Terrain { get; private set; } = string.Empty;
        public string Owner { get; private set; } = string.Empty;
        public string Controller { get; private set; } = string.Empty;

        public ProvinceInfo(MainWindow motherWindow, MasterController controller)
        {
            motherWindow_ = motherWindow;
            controller_ = controller;
        }

        public void HandleProvinceChanged(object? sender, ProvinceEventArgs e)
        {
            UpdateCurrentProvince(controller_.tickHandler.GetState(), e.ProvinceId);
        }

        public void UpdateCurrentProvince(WorldState state)
        {
            if (currentProvinceId_ >= 0)
                UpdateCurrentProvince(state, currentProvinceId_);
        }

        public void UpdateCurrentProvince(WorldState state, int provinceId)
        {
            if (state == null)
                return;
            var table = state.GetProvinceTable();
            if (table == null)
                return;

            if (!table.TryGetValue(provinceId, out var prov))
                return;
            currentProvinceId_ = provinceId;

            // Basic province info from base Province
            motherWindow_.ProvinceName.Text = prov.Name ?? string.Empty;
            motherWindow_.ProvinceOwner.Text = prov.Owner ?? string.Empty;
            motherWindow_.ProvinceController.Text = prov.Controller ?? string.Empty;
            motherWindow_.ProvincePopulation.Text = ((CwpProvince)prov).Population.ToString();

            // Game-specific extended province info may live in derived type CwpProvince
            if (prov is CwpProvince cwp)
            {
                Population = cwp.Population;
                Industrialization = cwp.Industrialization;
                Education = cwp.Education;
                Production = cwp.Production;
                Terrain = cwp.Terrain ?? string.Empty;
            }
            else
            {
                // Defaults when extended data not present
                Population = 0;
                Industrialization = 0;
                Education = 0;
                Production = 0;
                Terrain = string.Empty;
            }

            // Try to apply to form controls if they exist (non-breaking; uses reflection)
            TryApplyToFormControl("ProvinceName", Name);
            TryApplyToFormControl("ProvincePopulation", Population.ToString());
            TryApplyToFormControl("ProvinceIndustrialization", Industrialization.ToString());
            TryApplyToFormControl("ProvinceEducation", Education.ToString());
            TryApplyToFormControl("ProvinceProduction", Production.ToString());
            TryApplyToFormControl("ProvinceTerrain", Terrain);
            TryApplyToFormControl("ProvinceOwner", Owner);
            TryApplyToFormControl("ProvinceController", Controller);
        }

        private void TryApplyToFormControl(string controlName, string text)
        {
            if (motherWindow_ == null)
                return;
            var t = motherWindow_.GetType();

            // try field first
            var fi = t.GetField(
                controlName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            if (fi != null)
            {
                var obj = fi.GetValue(motherWindow_);
                if (obj is System.Windows.Forms.Control c)
                {
                    c.Text = text;
                    return;
                }
            }

            // try property
            var pi = t.GetProperty(
                controlName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            if (pi != null)
            {
                var obj = pi.GetValue(motherWindow_);
                if (obj is System.Windows.Forms.Control c)
                {
                    c.Text = text;
                }
            }
        }
    }
}
