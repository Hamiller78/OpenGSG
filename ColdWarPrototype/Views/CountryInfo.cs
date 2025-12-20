using System;
using System.ComponentModel;
using ColdWarGameLogic.GameLogic;
using ColdWarGameLogic.GameWorld;
using ColdWarGameLogic.GameWorld.ViewModels;
using ColdWarPrototype.Controller;
using ColdWarPrototype2;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;

namespace ColdWarPrototype.Views
{
    /// <summary>
    /// Minimal adapter between WinForms MainWindow and CwpCountryViewModel.
    /// Display logic should live in the ViewModel; this class only wires the ViewModel to form controls
    /// and ensures event handlers are detached when switching countries or disposing.
    /// </summary>
    public class CountryInfo : IDisposable
    {
        private readonly MainWindow motherWindow_;
        private readonly MasterController gameController_;
        private string currentTag_ = string.Empty;
        private CwpCountryViewModel? viewModel_;

        // Exposed data fields (kept for compatibility)
        public string CurrentTag { get; private set; } = string.Empty;
        public string CurrentName { get; private set; } = string.Empty;
        public (byte R, byte G, byte B)? CurrentColor { get; private set; } = null;

        public CountryInfo(MainWindow motherWindow, MasterController controller)
        {
            motherWindow_ = motherWindow ?? throw new ArgumentNullException(nameof(motherWindow));
            gameController_ = controller ?? throw new ArgumentNullException(nameof(controller));

            // Subscribe to the static TickHandler UI refresh request so the adapter can refresh the view
            TickHandler.UIRefreshRequested += TickHandler_UIRefreshRequested;
        }

        // Handler invoked when the mouse controller signals a country change
        public void HandleCountryChanged(object? sender, CountryEventArgs e)
        {
            UpdateCountryInfo(gameController_.TickHandler.GetState(), e.CountryTag);
        }

        public void UpdateCurrentCountry(WorldState state)
        {
            if (!string.IsNullOrEmpty(currentTag_))
                UpdateCountryInfo(state, currentTag_);
        }

        public void UpdateCountryInfo(WorldState state, string countryTag)
        {
            if (state == null || string.IsNullOrEmpty(countryTag))
                return;

            var table = state.GetCountryTable();
            if (table == null || !table.TryGetValue(countryTag, out var country))
                return;

            currentTag_ = countryTag;

            // Detach old ViewModel if present
            DetachViewModel();

            // Base properties (from Country)
            CurrentTag = country.Tag ?? string.Empty;
            CurrentName = country.Name ?? string.Empty;
            var c = country.Color;
            CurrentColor = c != null ? (c.Item1, c.Item2, c.Item3) : null;

            if (country is CwpCountry cwpCountry)
            {
                // Create ViewModel and subscribe to updates
                viewModel_ = new CwpCountryViewModel(cwpCountry);
                viewModel_.PropertyChanged += ViewModel_PropertyChanged;

                // Initialize UI from ViewModel (ViewModel contains display logic)
                RefreshAllFromViewModel();
            }
            else
            {
                // No extended data -> clear extended controls
                SetTextSafe(() => motherWindow_.CountryName.Text = CurrentName);
                SetTextSafe(() => motherWindow_.CountryLeader.Text = string.Empty);
                SetTextSafe(() => motherWindow_.CountryGovernment.Text = string.Empty);
                SetTextSafe(() => motherWindow_.CountryAllegiance.Text = string.Empty);
                SetTextSafe(() => motherWindow_.CountryProduction.Text = string.Empty);
                SetImageSafe(() => motherWindow_.FlagPictureBox.Image = country.Flag);
            }
        }

        private void TickHandler_UIRefreshRequested(object? sender, TickEventArgs e)
        {
            // Called after TickDone subscribers have updated the models.
            // Best-effort refresh strategy:
            //  - If we have a ViewModel for the current country, call Refresh() and update UI from it.
            //  - Otherwise, if we know the current country tag, re-read country data via UpdateCountryInfo.
            if (viewModel_ != null)
            {
                // Pull model values into the VM and refresh UI
                viewModel_.Refresh();
                RefreshAllFromViewModel();
            }
            else if (!string.IsNullOrEmpty(currentTag_))
            {
                // Re-apply current country data from world state (handles non-CwpCountry and initial state)
                var state = gameController_.TickHandler.GetState();
                UpdateCountryInfo(state, currentTag_);
            }
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e?.PropertyName == null || viewModel_ == null)
                return;

            // Update backing fields and UI for the changed property. Keep this mapping minimal.
            switch (e.PropertyName)
            {
                case nameof(CwpCountryViewModel.FullName):
                    CurrentName = viewModel_.FullName;
                    SetTextSafe(() => motherWindow_.CountryName.Text = CurrentName);
                    break;

                case nameof(CwpCountryViewModel.Leader):
                    SetTextSafe(
                        () => motherWindow_.CountryLeader.Text = viewModel_.Leader ?? string.Empty
                    );
                    break;

                case nameof(CwpCountryViewModel.Government):
                    SetTextSafe(
                        () =>
                            motherWindow_.CountryGovernment.Text =
                                viewModel_.Government ?? string.Empty
                    );
                    break;

                case nameof(CwpCountryViewModel.Allegiance):
                    SetTextSafe(
                        () =>
                            motherWindow_.CountryAllegiance.Text =
                                viewModel_.Allegiance ?? string.Empty
                    );
                    break;

                case nameof(CwpCountryViewModel.Flag):
                    SetImageSafe(() => motherWindow_.FlagPictureBox.Image = viewModel_.Flag);
                    break;

                case nameof(CwpCountryViewModel.Name):
                    CurrentName = viewModel_.Name ?? string.Empty;
                    SetTextSafe(() => motherWindow_.CountryName.Text = CurrentName);
                    break;

                case nameof(CwpCountryViewModel.Tag):
                    CurrentTag = viewModel_.Tag ?? string.Empty;
                    break;

                case nameof(CwpCountryViewModel.Color):
                    var c = viewModel_.Color;
                    CurrentColor = c != null ? (c.Item1, c.Item2, c.Item3) : null;
                    break;
            }
        }

        private void RefreshAllFromViewModel()
        {
            if (viewModel_ == null)
                return;

            CurrentName = viewModel_.FullName ?? string.Empty;
            CurrentTag = viewModel_.Tag ?? string.Empty;
            var c = viewModel_.Color;
            CurrentColor = c != null ? (c.Item1, c.Item2, c.Item3) : null;

            SetTextSafe(() => motherWindow_.CountryName.Text = viewModel_.FullName ?? string.Empty);
            SetTextSafe(() => motherWindow_.CountryLeader.Text = viewModel_.Leader ?? string.Empty);
            SetTextSafe(
                () => motherWindow_.CountryGovernment.Text = viewModel_.Government ?? string.Empty
            );
            SetTextSafe(
                () => motherWindow_.CountryAllegiance.Text = viewModel_.Allegiance ?? string.Empty
            );
            // TODO: Calculate and display country production when available
            SetTextSafe(() => motherWindow_.CountryProduction.Text = string.Empty);
            SetImageSafe(() => motherWindow_.FlagPictureBox.Image = viewModel_.Flag);
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

        private void SetImageSafe(Action uiAction)
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
            // Unsubscribe from static TickHandler refresh event
            TickHandler.UIRefreshRequested -= TickHandler_UIRefreshRequested;

            DetachViewModel();
            GC.SuppressFinalize(this);
        }
    }
}
