using System;
using System.ComponentModel;
using ColdWarGameLogic.GameLogic;
using ColdWarGameLogic.GameWorld;
using ColdWarGameLogic.GameWorld.ViewModels;
using ColdWarPrototype.Controller;
using ColdWarPrototype2;
using OpenGSGLibrary.GameDataManager;

namespace ColdWarPrototype.Views
{
    /// <summary>
    /// Minimal adapter between WinForms MainWindow and CwpProvinceViewModel.
    /// Display logic should live in the ViewModel; this class only wires the ViewModel to form controls
    /// and ensures event handlers are detached when switching provinces or disposing.
    /// </summary>
    public class ProvinceInfo : IDisposable
    {
        private readonly MainWindow motherWindow_;
        private readonly MasterController controller_;
        private int currentProvinceId_ = -1;
        private CwpProvinceViewModel? viewModel_;

        // Exposed data fields (kept for compatibility)
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
            motherWindow_ = motherWindow ?? throw new ArgumentNullException(nameof(motherWindow));
            controller_ = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        public void HandleProvinceChanged(object? sender, ProvinceEventArgs e)
        {
            UpdateCurrentProvince(controller_.TickHandler.GetState(), e.ProvinceId);
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

            // Detach old ViewModel if present
            DetachViewModel();

            // Base properties (from Province)
            Name = prov.Name ?? string.Empty;
            Owner = prov.Owner ?? string.Empty;
            Controller = prov.Controller ?? string.Empty;

            // Immediately update base controls
            SetTextSafe(() => motherWindow_.ProvinceName.Text = Name);
            SetTextSafe(() => motherWindow_.ProvinceOwner.Text = Owner);
            SetTextSafe(() => motherWindow_.ProvinceController.Text = Controller);

            if (prov is CwpProvince cwp)
            {
                // Create ViewModel and subscribe to updates
                viewModel_ = new CwpProvinceViewModel(cwp);
                viewModel_.PropertyChanged += ViewModel_PropertyChanged;

                // Initialize UI from ViewModel (ViewModel contains display logic)
                RefreshAllFromViewModel();
            }
            else
            {
                // No extended data -> clear extended controls
                Population = 0;
                Industrialization = 0;
                Education = 0;
                Production = 0;
                Terrain = string.Empty;

                SetTextSafe(() => motherWindow_.ProvincePopulation.Text = Population.ToString());
                SetTextSafe(
                    () =>
                        motherWindow_.ProvinceIndustrialization.Text = Industrialization.ToString()
                );
                SetTextSafe(() => motherWindow_.ProvinceEducation.Text = Education.ToString());
                SetTextSafe(() => motherWindow_.ProvinceProduction.Text = Production.ToString());
                SetTextSafe(() => motherWindow_.ProvinceTerrain.Text = Terrain);
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e?.PropertyName == null || viewModel_ == null)
                return;

            // Update backing fields and UI for the changed property. Keep this mapping minimal.
            switch (e.PropertyName)
            {
                case nameof(CwpProvinceViewModel.Population):
                    Population = viewModel_.Population;
                    SetTextSafe(
                        () => motherWindow_.ProvincePopulation.Text = Population.ToString()
                    );
                    break;

                case nameof(CwpProvinceViewModel.Industrialization):
                    Industrialization = viewModel_.Industrialization;
                    SetTextSafe(
                        () =>
                            motherWindow_.ProvinceIndustrialization.Text =
                                Industrialization.ToString()
                    );
                    break;

                case nameof(CwpProvinceViewModel.Education):
                    Education = viewModel_.Education;
                    SetTextSafe(() => motherWindow_.ProvinceEducation.Text = Education.ToString());
                    break;

                case nameof(CwpProvinceViewModel.Terrain):
                    Terrain = viewModel_.Terrain ?? string.Empty;
                    SetTextSafe(() => motherWindow_.ProvinceTerrain.Text = Terrain);
                    break;

                case nameof(CwpProvinceViewModel.Production):
                    Production = viewModel_.Production;
                    SetTextSafe(
                        () => motherWindow_.ProvinceProduction.Text = Production.ToString()
                    );
                    break;

                // Forwarded base properties (ViewModel forwards model changes)
                case nameof(CwpProvinceViewModel.Name):
                    Name = viewModel_.Model.Name ?? string.Empty;
                    SetTextSafe(() => motherWindow_.ProvinceName.Text = Name);
                    break;

                case nameof(CwpProvinceViewModel.Owner):
                    Owner = viewModel_.Model.Owner ?? string.Empty;
                    SetTextSafe(() => motherWindow_.ProvinceOwner.Text = Owner);
                    break;

                case nameof(CwpProvinceViewModel.Controller):
                    Controller = viewModel_.Model.Controller ?? string.Empty;
                    SetTextSafe(() => motherWindow_.ProvinceController.Text = Controller);
                    break;

                default:
                    // For other property names, conservatively refresh Production if inputs changed.
                    if (
                        e.PropertyName == nameof(CwpProvinceViewModel.Population)
                        || e.PropertyName == nameof(CwpProvinceViewModel.Industrialization)
                        || e.PropertyName == nameof(CwpProvinceViewModel.Education)
                    )
                    {
                        Production = viewModel_.Production;
                        SetTextSafe(
                            () => motherWindow_.ProvinceProduction.Text = Production.ToString()
                        );
                    }
                    break;
            }
        }

        private void RefreshAllFromViewModel()
        {
            if (viewModel_ == null)
                return;

            Population = viewModel_.Population;
            Industrialization = viewModel_.Industrialization;
            Education = viewModel_.Education;
            Production = viewModel_.Production;
            Terrain = viewModel_.Terrain ?? string.Empty;

            SetTextSafe(() => motherWindow_.ProvincePopulation.Text = Population.ToString());
            SetTextSafe(
                () => motherWindow_.ProvinceIndustrialization.Text = Industrialization.ToString()
            );
            SetTextSafe(() => motherWindow_.ProvinceEducation.Text = Education.ToString());
            SetTextSafe(() => motherWindow_.ProvinceProduction.Text = Production.ToString());
            SetTextSafe(() => motherWindow_.ProvinceTerrain.Text = Terrain);
        }

        private void DetachViewModel()
        {
            if (viewModel_ == null)
                return;
            viewModel_.PropertyChanged -= ViewModel_PropertyChanged;
            viewModel_.Dispose();
            viewModel_ = null;
        }

        // Ensure UI updates happen on the UI thread (WinForms).
        private void SetTextSafe(Action uiAction)
        {
            if (motherWindow_.InvokeRequired)
            {
                motherWindow_.BeginInvoke(uiAction);
            }
            else
            {
                uiAction();
            }
        }

        public void Dispose()
        {
            DetachViewModel();
            GC.SuppressFinalize(this);
        }
    }
}
