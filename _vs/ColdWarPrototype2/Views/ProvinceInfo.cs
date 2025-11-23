using System;
using System.Reflection;
using Simulation;
using WorldData;

namespace ColdWarPrototype2.Views
{
    public class ProvinceInfo
    {
        private readonly Form1 motherWindow_;
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

        public ProvinceInfo(Form1 motherWindow, MasterController controller)
        {
            motherWindow_ = motherWindow;
            controller_ = controller;
        }

        // Single weakly-typed handler that accepts any event args and extracts province id via reflection
        public void HandleProvinceChanged(object? sender, object? e)
        {
            if (e == null) return;

            // Try direct property names first (ProvinceId / provinceId)
            var et = e.GetType();
            var prop = et.GetProperty("ProvinceId") ?? et.GetProperty("provinceId");
            if (prop != null)
            {
                var v = prop.GetValue(e);
                if (v != null && int.TryParse(v.ToString(), out var id))
                {
                    UpdateCurrentProvince(controller_.tickHandler.GetState(), id);
                    return;
                }
            }

            // Fallback: try fields with the same names
            var field = et.GetField("ProvinceId") ?? et.GetField("provinceId");
            if (field != null)
            {
                var v = field.GetValue(e);
                if (v != null && int.TryParse(v.ToString(), out var id))
                {
                    UpdateCurrentProvince(controller_.tickHandler.GetState(), id);
                }
            }
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
            Name = prov.GetName() ?? string.Empty;
            Owner = prov.GetOwner() ?? string.Empty;
            Controller = prov.GetController() ?? string.Empty;

            // Game-specific extended province info may live in derived type CwpProvince
            if (prov is CwpProvince cwp)
            {
                Population = cwp.population;
                Industrialization = cwp.industrialization;
                Education = cwp.education;
                Production = cwp.production;
                Terrain = cwp.terrain ?? string.Empty;
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
