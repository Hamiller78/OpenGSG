using ColdWarGameLogic.GameLogic;
using ColdWarPrototype;
using ColdWarPrototype.Controller;
using ColdWarPrototype.Dialogs;
using ColdWarPrototype.Views;
using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;
using OpenGSGLibrary.Tools;
using OpenGSGLibrary.WorldMap;

namespace ColdWarPrototype2
{
    public partial class MainWindow : Form
    {
        private Logger log;
        private MasterController _gameController = new();

        // view helpers
        private ProvinceInfo? _provinceInfo;
        private CountryInfo? _countryInfo;
        private ArmyList? _armyBox;
        private GeoCoordinates _coordinateView;
        private WorldMap _worldMapView;
        private DiplomacyInfo? _diplomacyInfo;
        private ActiveCountryInfo? _activeCountryInfo;
        private MilitaryUnitsView? _militaryUnitsView; // ADD THIS

        // Controllers
        private MouseController _mouseController;

        private DebugConsole? _debugConsole;

        // Pin state
        private int _pinnedProvinceId = -1; // -1 = not pinned
        private bool IsProvincePinned => _pinnedProvinceId > 0;

        // Add field
        private System.Windows.Forms.Timer? _blinkTimer;

        public MainWindow()
        {
            InitializeComponent();
            log = new Logger("CWPLog.log", Directory.GetCurrentDirectory());
            Load += MainWindow_Load;
        }

        private void MainWindow_Load(object? sender, EventArgs e)
        {
            log.WriteLine(LogLevel.Info, "Session started.");
            try
            {
                _gameController.Init();

                // Create debug console (hidden by default)
                _debugConsole = new DebugConsole(_gameController);

                // Wire up the console button
                DebugConsoleButton.Click += (s, e) => _debugConsole?.Toggle();

                SetupViews();
                SetupControllers();
                SetupEventHandlers();
                SetupSimulationControls();

                UpdateDateText();

                // Load province map and set on picture box
                var provinceMap = _gameController.WorldData.ProvinceMap;
                if (provinceMap?.sourceBitmap != null)
                {
                    MapPictureBox.Image = new Bitmap(provinceMap.sourceBitmap);
                    MapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
                }

                MapPictureBox.MouseMove += MapPictureBox_MouseMove;
                MapPictureBox.MouseClick += MapPictureBox_MouseClick; // ADD THIS
                DateButton.Text = _gameController.GetGameDateTime().ToString();
                DateButton.Click += DateButton_Click;
                MapModePolitical.CheckedChanged += MapModePolitical_CheckedChanged;
                MapModeRaw.CheckedChanged += MapModeRaw_CheckedChanged;

                var worldMapView = new WorldMap(this);
                worldMapView.SetSourceProvinceMap(provinceMap);
                worldMapView.UpdateCountryMap(_gameController.TickHandler.GetState());

                // Subscribe to event triggers
                TickHandler.EventTriggered += TickHandler_EventTriggered;

                // Subscribe to UI refresh events (updates date display automatically)
                TickHandler.UIRefreshRequested += TickHandler_UIRefreshRequested;

                // Add this after setting up map
                _blinkTimer = new System.Windows.Forms.Timer();
                _blinkTimer.Interval = 500; // Blink every 500ms
                _blinkTimer.Tick += BlinkTimer_Tick;
                _blinkTimer.Start();
            }
            catch (Exception ex)
            {
                log.WriteLine(LogLevel.Err, "Error initializing game controller: " + ex.Message);
            }
        }

        private void SetupViews()
        {
            _provinceInfo = new ProvinceInfo(this, _gameController);
            _countryInfo = new CountryInfo(this, _gameController);
            _armyBox = new ArmyList(this);
            _coordinateView = new GeoCoordinates(this);
            _diplomacyInfo = new DiplomacyInfo(this, _gameController);
            _activeCountryInfo = new ActiveCountryInfo(this, _gameController);
            _militaryUnitsView = new MilitaryUnitsView(this); // ADD THIS

            _worldMapView = new WorldMap(this);
            _worldMapView.SetSourceProvinceMap(_gameController.WorldData.ProvinceMap);
            _worldMapView.UpdateCountryMap(_gameController.TickHandler.GetState());
        }

        private void SetupControllers()
        {
            _mouseController = new MouseController(_gameController);
            Size sourceMapSize = _gameController.WorldData.ProvinceMap.sourceBitmap.Size;
            _mouseController.SetMapScalingFactor(MapPictureBox.Size, sourceMapSize);
        }

        private void SetupEventHandlers()
        {
            // Province hover - only update if not pinned
            _mouseController.HoveredProvinceChanged += (sender, e) =>
            {
                if (!IsProvincePinned && e.ProvinceId > 0)
                {
                    UpdateProvinceViews(e.ProvinceId);
                }
            };

            // Province click - toggle pin
            _mouseController.ProvinceClicked += (sender, e) =>
            {
                if (_pinnedProvinceId == e.ProvinceId)
                {
                    // Unpin - clicked on same province
                    _pinnedProvinceId = -1;
                    // Immediately update to current hover province
                    UpdateProvinceViews(e.ProvinceId);
                }
                else
                {
                    // Pin to new province
                    _pinnedProvinceId = e.ProvinceId;
                    UpdateProvinceViews(e.ProvinceId);
                }
            };

            // Country hover - only update if not pinned
            _mouseController.HoveredCountryChanged += (sender, e) =>
            {
                if (!IsProvincePinned)
                {
                    _countryInfo?.HandleCountryChanged(sender, e);
                    _diplomacyInfo?.HandleCountryChanged(sender, e);
                }
            };

            // Military tab - populate list when tab is selected
            ActiveCountryTabControl.SelectedIndexChanged += (sender, e) =>
            {
                if (ActiveCountryTabControl.SelectedTab == MilitaryTabPage)
                {
                    UpdateMilitaryTab();
                }
            };

            // Military list - update button state when selection changes
            MilitaryUnitsListView.SelectedIndexChanged += (sender, e) =>
            {
                bool hasSelection = MilitaryUnitsListView.SelectedItems.Count > 0;
                _militaryUnitsView?.UpdateMoveButtonState(IsProvincePinned, hasSelection);
            };

            // Move button click
            MoveUnitsButton.Click += MoveUnitsButton_Click;
        }

        private void UpdateMilitaryTab()
        {
            var activeTag = _gameController.TickHandler.ActivePlayerCountryTag;
            _militaryUnitsView?.UpdateMilitaryList(
                _gameController.TickHandler.GetState(),
                activeTag
            );
        }

        private void MoveUnitsButton_Click(object? sender, EventArgs e)
        {
            var formation = _militaryUnitsView?.GetSelectedFormation();
            if (formation == null || !IsProvincePinned)
                return;

            // TODO: Create MarchOrder and add to order queue
            MessageBox.Show(
                $"Moving {formation.Branch} to province {_pinnedProvinceId}\n"
                    + $"(Order system not yet implemented)",
                "Move Order",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Updates all province-related views (province info, country info, army list).
        /// </summary>
        private void UpdateProvinceViews(int provinceId)
        {
            if (provinceId <= 0)
                return;

            var state = _gameController.TickHandler.GetState();

            // Update province info
            _provinceInfo?.HandleProvinceChanged(this, new ProvinceEventArgs(provinceId));

            // Update army list
            _armyBox?.UpdateArmyListBox(state, provinceId);

            // Update country info based on province owner
            var provinceTable = state.GetProvinceTable();
            if (provinceTable != null && provinceTable.TryGetValue(provinceId, out var province))
            {
                if (!string.IsNullOrEmpty(province.Owner))
                {
                    _countryInfo?.HandleCountryChanged(this, new CountryEventArgs(province.Owner));
                    _diplomacyInfo?.HandleCountryChanged(
                        this,
                        new CountryEventArgs(province.Owner)
                    );
                }
            }

            // Set highlighted province for blinking
            if (_worldMapView?.MapMaker is CountryModeMapMaker countryMap)
            {
                countryMap.HighlightedProvinceId = IsProvincePinned ? _pinnedProvinceId : -1;

                // When unpinning, reset to normal color immediately
                if (!IsProvincePinned)
                {
                    countryMap.ShowHighlight = false;
                    // Force redraw to show normal color (works in both modes)
                    _worldMapView.UpdateCountryMap(state);
                }
            }

            // Update move button state
            bool hasSelection = MilitaryUnitsListView.SelectedItems.Count > 0;
            _militaryUnitsView?.UpdateMoveButtonState(IsProvincePinned, hasSelection);
        }

        private void SetupSimulationControls()
        {
            // Setup Play/Pause button
            PlayPauseButton.Text = "▶ Play";
            PlayPauseButton.Click += PlayPauseButton_Click;

            // Setup speed dropdown with custom labels
            SpeedComboBox.Items.AddRange(
                new object[]
                {
                    "1x - Very Slow",
                    "2x - Slow",
                    "3x - Normal",
                    "4x - Fast",
                    "5x - Very Fast",
                    "Max Speed",
                }
            );
            SpeedComboBox.SelectedIndex = 2; // Normal
            SpeedComboBox.SelectedIndexChanged += SpeedComboBox_SelectedIndexChanged;

            // Subscribe to simulation events
            _gameController.SimulationThread.SimulationStarted += (s, e) => UpdatePlayPauseButton();
            _gameController.SimulationThread.SimulationPaused += (s, e) => UpdatePlayPauseButton();
            _gameController.SimulationThread.SimulationResumed += (s, e) => UpdatePlayPauseButton();
        }

        private void MapPictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            _mouseController.HandleMouseMovedOverMap(e);
        }

        private void MapPictureBox_MouseClick(object? sender, MouseEventArgs e)
        {
            _mouseController.HandleMouseClickOnMap(e);
        }

        private void MapModePolitical_CheckedChanged(object? sender, EventArgs e)
        {
            _worldMapView?.SetMapPicture();
        }

        private void MapModeRaw_CheckedChanged(object? sender, EventArgs e)
        {
            _worldMapView?.SetMapPicture();
        }

        private void PlayPauseButton_Click(object? sender, EventArgs e)
        {
            var sim = _gameController.SimulationThread;

            if (!sim.IsRunning)
            {
                sim.Start();
            }
            else if (sim.IsPaused)
            {
                sim.Resume();
            }
            else
            {
                sim.Pause();
            }

            UpdatePlayPauseButton();
        }

        private void UpdatePlayPauseButton()
        {
            if (InvokeRequired)
            {
                BeginInvoke(UpdatePlayPauseButton);
                return;
            }

            var sim = _gameController.SimulationThread;
            PlayPauseButton.Text = sim.IsRunning && !sim.IsPaused ? "⏸ Pause" : "▶ Play";
        }

        private void SpeedComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Explicit mapping from ComboBox index to enum
            var speed = SpeedComboBox.SelectedIndex switch
            {
                0 => SimulationSpeed.VerySlow,
                1 => SimulationSpeed.Slow,
                2 => SimulationSpeed.Normal,
                3 => SimulationSpeed.Fast,
                4 => SimulationSpeed.VeryFast,
                5 => SimulationSpeed.Maximum,
                _ => SimulationSpeed.Normal,
            };

            _gameController.SimulationThread.SetSpeed(speed);
        }

        // NEW: Update UI automatically when tick completes
        private void TickHandler_UIRefreshRequested(object? sender, TickEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => TickHandler_UIRefreshRequested(sender, e));
                return;
            }

            UpdateDateText();
            _provinceInfo?.UpdateCurrentProvince(_gameController.TickHandler.GetState());
            _countryInfo?.UpdateCurrentCountry(_gameController.TickHandler.GetState());
            _diplomacyInfo?.UpdateCurrentCountry(_gameController.TickHandler.GetState());
            _activeCountryInfo?.UpdateActiveCountry(_gameController.TickHandler.GetState());

            // Refresh military tab if it's active
            if (ActiveCountryTabControl.SelectedTab == MilitaryTabPage)
            {
                UpdateMilitaryTab();
            }
        }

        // MODIFIED: DateButton now toggles pause instead of advancing manually
        private void DateButton_Click(object? sender, EventArgs e)
        {
            PlayPauseButton_Click(sender, e); // Reuse play/pause logic
        }

        private void UpdateDateText()
        {
            DateButton.Text = _gameController.GetGameDateTime().ToString();
        }

        private void TickHandler_EventTriggered(object? sender, GameEventTriggeredEventArgs e)
        {
            // Marshal to UI thread if needed
            if (InvokeRequired)
            {
                BeginInvoke(() => TickHandler_EventTriggered(sender, e));
                return;
            }

            // Auto-pause the game before showing the event
            var sim = _gameController.SimulationThread;
            bool wasRunning = sim.IsRunning && !sim.IsPaused;

            if (wasRunning)
            {
                sim.Pause();
            }

            // Now we're on the UI thread - safe to show dialog
            // Create and show event dialog as a modal dialog
            using var eventDialog = new EventDialog();
            eventDialog.Owner = this;
            eventDialog.ShowEvent(
                e.Event,
                e.Context,
                _gameController.LocalizationManager,
                () =>
                {
                    // Event completed callback
                    // Auto-resume if the game was running before the event
                    if (wasRunning)
                    {
                        sim.Resume();
                    }
                }
            );

            // Also resume here (in case dialog is closed without clicking an option)
            // This won't double-resume because Resume() checks if already running
            if (wasRunning && sim.IsPaused)
            {
                sim.Resume();
            }
        }

        // NEW: Key handling for console toggle
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // F12 to toggle console (universal across keyboard layouts)
            if (keyData == Keys.F12)
            {
                _debugConsole?.Toggle();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop simulation thread cleanly
            _gameController.SimulationThread?.Stop();
            base.OnFormClosing(e);
        }

        private void BlinkTimer_Tick(object? sender, EventArgs e)
        {
            // Only blink if something is pinned (works in both modes now)
            if (!IsProvincePinned)
                return;

            if (_worldMapView?.MapMaker is CountryModeMapMaker countryMap)
            {
                countryMap.ShowHighlight = !countryMap.ShowHighlight;

                if (countryMap.HighlightedProvinceId > 0)
                {
                    _worldMapView.UpdateCountryMap(_gameController.TickHandler.GetState());
                }
            }
        }

        // Add this public accessor method
        public MasterController GetGameController() => _gameController;
    }
}
