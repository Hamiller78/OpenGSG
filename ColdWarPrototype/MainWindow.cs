using ColdWarGameLogic.GameLogic;
using ColdWarPrototype;
using ColdWarPrototype.Controller;
using ColdWarPrototype.Dialogs;
using ColdWarPrototype.Views;
using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameLogic;
using OpenGSGLibrary.Tools;

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

        // Controllers
        private MouseController _mouseController;

        private DebugConsole? _debugConsole;

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
            _diplomacyInfo = new DiplomacyInfo(this, _gameController); // ← Add this

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
            _mouseController.HoveredProvinceChanged += _provinceInfo.HandleProvinceChanged;
            _mouseController.HoveredCountryChanged += _countryInfo.HandleCountryChanged; // ← Add this back
            _mouseController.HoveredCountryChanged += _diplomacyInfo.HandleCountryChanged; // ← Keep this
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

        private void TickHandler_EventTriggered(object? sender, EventTriggeredArgs e)
        {
            // Create and show event dialog as a modal dialog
            using var eventDialog = new EventDialog();
            eventDialog.ShowEvent(
                e.Event,
                e.Context,
                _gameController.LocalizationManager,
                () => {
                    // Event completed callback - can resume game or process queue
                }
            );
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
    }
}
