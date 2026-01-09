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

        // Controllers
        private MouseController mouseController_;

        private EventDialog? eventDialog_;

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

                UpdateDateText();

                // Load province map and set on picture box
                var provinceMap = gameController_.WorldData.ProvinceMap;
                if (provinceMap?.sourceBitmap != null)
                {
                    MapPictureBox.Image = new Bitmap(provinceMap.sourceBitmap);
                    MapPictureBox.SizeMode = PictureBoxSizeMode.Normal;
                }

                // create views
                provinceInfo_ = new ProvinceInfo(this, gameController_);
                countryInfo_ = new CountryInfo(this, gameController_);
                armyBox_ = new ArmyList(this);

                MapPictureBox.MouseMove += MapPictureBox_MouseMove;
                DateButton.Text = gameController_.GetGameDateTime().ToString();
                DateButton.Click += DateButton_Click;
                MapModePolitical.CheckedChanged += MapModePolitical_CheckedChanged;
                MapModeRaw.CheckedChanged += MapModeRaw_CheckedChanged;

                var worldMapView = new WorldMap(this);
                worldMapView.SetSourceProvinceMap(provinceMap);
                worldMapView.UpdateCountryMap(gameController_.TickHandler.GetState());

                // Create event dialog
                eventDialog_ = new EventDialog();
                eventDialog_.Visible = false;
                eventDialog_.Location = new Point(
                    (this.ClientSize.Width - eventDialog_.Width) / 2,
                    (this.ClientSize.Height - eventDialog_.Height) / 2
                );
                this.Controls.Add(eventDialog_);
                eventDialog_.BringToFront();

                // Subscribe to event triggers
                TickHandler.EventTriggered += TickHandler_EventTriggered;
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
            mouseController_.HoveredCountryChanged += countryInfo_.HandleCountryChanged;
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

        private void DateButton_Click(object? sender, EventArgs e)
        {
            gameController_.TickHandler.FinishTick();

            UpdateDateText();
            provinceInfo_?.UpdateCurrentProvince(gameController_.TickHandler.GetState());
            countryInfo_?.UpdateCurrentCountry(gameController_.TickHandler.GetState());
        }

        private void UpdateDateText()
        {
            DateButton.Text = gameController_.GetGameDateTime().ToString();
        }

        private void TickHandler_EventTriggered(object? sender, EventTriggeredArgs e)
        {
            // Display event to player
            if (eventDialog_ != null)
            {
                eventDialog_.ShowEvent(
                    e.Event,
                    e.Context,
                    () => {
                        // Event completed callback - can resume game or process queue
                    }
                );
            }
        }
    }
}
