using ColdWarGameLogic.GameLogic;
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
        private MasterController gameController_ = new();

        // view helpers
        private ProvinceInfo? provinceInfo_;
        private CountryInfo? countryInfo_;
        private ArmyList? armyBox_;
        private GeoCoordinates coordinateView_;
        private WorldMap worldMapView_;
        private DiplomacyInfo? diplomacyInfo_;

        // Controllers
        private MouseController mouseController_;

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
                gameController_.Init();

                SetupViews();
                SetupControllers();
                SetupEventHandlers();
                SetupSimulationControls();

                UpdateDateText();

                // Load province map and set on picture box
                var provinceMap = gameController_.WorldData.ProvinceMap;
                if (provinceMap?.sourceBitmap != null)
                {
                    MapPictureBox.Image = new Bitmap(provinceMap.sourceBitmap);
                    MapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
                }

                MapPictureBox.MouseMove += MapPictureBox_MouseMove;
                DateButton.Text = gameController_.GetGameDateTime().ToString();
                DateButton.Click += DateButton_Click;
                MapModePolitical.CheckedChanged += MapModePolitical_CheckedChanged;
                MapModeRaw.CheckedChanged += MapModeRaw_CheckedChanged;

                var worldMapView = new WorldMap(this);
                worldMapView.SetSourceProvinceMap(provinceMap);
                worldMapView.UpdateCountryMap(gameController_.TickHandler.GetState());

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
            provinceInfo_ = new ProvinceInfo(this, gameController_);
            countryInfo_ = new CountryInfo(this, gameController_);
            armyBox_ = new ArmyList(this);
            coordinateView_ = new GeoCoordinates(this);
            diplomacyInfo_ = new DiplomacyInfo(this, gameController_); // ← Add this

            worldMapView_ = new WorldMap(this);
            worldMapView_.SetSourceProvinceMap(gameController_.WorldData.ProvinceMap);
            worldMapView_.UpdateCountryMap(gameController_.TickHandler.GetState());
        }

        private void SetupControllers()
        {
            mouseController_ = new MouseController(gameController_);
            Size sourceMapSize = gameController_.WorldData.ProvinceMap.sourceBitmap.Size;
            mouseController_.SetMapScalingFactor(MapPictureBox.Size, sourceMapSize);
        }

        private void SetupEventHandlers()
        {
            mouseController_.HoveredProvinceChanged += provinceInfo_.HandleProvinceChanged;
            mouseController_.HoveredCountryChanged += countryInfo_.HandleCountryChanged; // ← Add this back
            mouseController_.HoveredCountryChanged += diplomacyInfo_.HandleCountryChanged; // ← Keep this
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
            gameController_.SimulationThread.SimulationStarted += (s, e) => UpdatePlayPauseButton();
            gameController_.SimulationThread.SimulationPaused += (s, e) => UpdatePlayPauseButton();
            gameController_.SimulationThread.SimulationResumed += (s, e) => UpdatePlayPauseButton();
        }

        private void MapPictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            CoordsLabel.Text = $"X: {e.X}, Y: {e.Y}";
            mouseController_.HandleMouseMovedOverMap(e);
        }

        private void MapModePolitical_CheckedChanged(object? sender, EventArgs e)
        {
            worldMapView_?.SetMapPicture();
        }

        private void MapModeRaw_CheckedChanged(object? sender, EventArgs e)
        {
            worldMapView_?.SetMapPicture();
        }

        private void PlayPauseButton_Click(object? sender, EventArgs e)
        {
            var sim = gameController_.SimulationThread;

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

            var sim = gameController_.SimulationThread;
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

            gameController_.SimulationThread.SetSpeed(speed);
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
            provinceInfo_?.UpdateCurrentProvince(gameController_.TickHandler.GetState());
            countryInfo_?.UpdateCurrentCountry(gameController_.TickHandler.GetState());
            diplomacyInfo_?.UpdateCurrentCountry(gameController_.TickHandler.GetState());
        }

        // MODIFIED: DateButton now toggles pause instead of advancing manually
        private void DateButton_Click(object? sender, EventArgs e)
        {
            PlayPauseButton_Click(sender, e); // Reuse play/pause logic
        }

        private void UpdateDateText()
        {
            DateButton.Text = gameController_.GetGameDateTime().ToString();
        }

        private void TickHandler_EventTriggered(object? sender, EventTriggeredArgs e)
        {
            // Create and show event dialog as a modal dialog
            using var eventDialog = new EventDialog();
            eventDialog.ShowEvent(
                e.Event,
                e.Context,
                gameController_.LocalizationManager,
                () => {
                    // Event completed callback - can resume game or process queue
                }
            );
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop simulation thread cleanly
            gameController_.SimulationThread?.Stop();
            base.OnFormClosing(e);
        }
    }
}
