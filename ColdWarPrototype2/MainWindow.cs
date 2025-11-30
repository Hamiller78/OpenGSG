using System;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata;
using System.Windows.Forms;
using ColdWarPrototype2.Views;
using Gui;
using Simulation;
using Tools;

namespace ColdWarPrototype2
{
    public partial class MainWindow : Form
    {
        private Logger log;
        private MasterController gameController_ = new MasterController();

        // view helpers
        private ProvinceInfo? provinceInfo_;
        private CountryInfo? countryInfo_;
        private ArmyList? armyBox_;
        private GeoCoordinates coordinateView_;
        private WorldMap worldMapView_;

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

                UpdateDateText();

                // Load province map and set on picture box
                var provinceMap = gameController_.GetWorldManager().provinceMap;
                if (provinceMap?.sourceBitmap != null)
                {
                    MapPictureBox.Image = new Bitmap(provinceMap.sourceBitmap);
                    MapPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }

                // create views
                provinceInfo_ = new Views.ProvinceInfo(this, gameController_);
                countryInfo_ = new Views.CountryInfo(this, gameController_);
                armyBox_ = new Views.ArmyList(this);

                // wire simple mouse handling directly: update views on hover
                MapPictureBox.MouseMove += (s, ev) =>
                {
                    var state = gameController_.tickHandler.GetState();
                    var mapX = (int)(
                        ev.X * ((double)provinceMap.sourceBitmap!.Width / MapPictureBox.Width)
                    );
                    var mapY = (int)(
                        ev.Y * ((double)provinceMap.sourceBitmap.Height / MapPictureBox.Height)
                    );
                    var rgb = provinceMap.GetPixelRgb(mapX, mapY);
                    var provinceId = provinceMap.GetProvinceNumber(rgb);
                    if (provinceId != -1)
                    {
                        provinceInfo_?.UpdateCurrentProvince(state, provinceId);
                        var provTable = state.GetProvinceTable();
                        if (provTable != null && provTable.TryGetValue(provinceId, out var prov))
                        {
                            var owner = prov.GetOwner();
                            countryInfo_?.UpdateCountryInfo(state, owner);
                            armyBox_?.FillListBox(ArmyListBox, state, provinceId);
                        }
                    }
                };
                DateButton.Text = gameController_.GetGameDateTime().ToString();
                DateButton.Click += DateButton_Click;
                MapModePolitical.CheckedChanged += (
                    s,
                    ev
                ) => { /* TODO: political map */
                };
                MapModeRaw.CheckedChanged += (
                    s,
                    ev
                ) => { /* TODO: raw map */
                };

                var worldMapView = new Views.WorldMap(this);
                worldMapView.SetSourceProvinceMap(provinceMap);
                worldMapView.UpdateCountryMap(gameController_.tickHandler.GetState());
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
            worldMapView_.SetSourceProvinceMap(gameController_.GetWorldManager().provinceMap); // ugly
            worldMapView_.UpdateCountryMap(gameController_.tickHandler.GetState()); // not much better
        }

        private void SetupControllers()
        {
            mouseController_ = new Gui.MouseController(gameController_);
            Size sourceMapSize = gameController_.worldData.provinceMap.sourceBitmap.Size;
            mouseController_.SetMapScalingFactor(MapPictureBox.Size, sourceMapSize);
        }

        private void SetupEventHandlers()
        {
            mouseController_.HoveredProvinceChanged += provinceInfo_.HandleProvinceChanged;
            mouseController_.HoveredCountryChanged += countryInfo_.HandleCountryChanged;
        }

        private void MapPictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
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
            gameController_.tickHandler.FinishTick();

            UpdateDateText();
            provinceInfo_?.UpdateCurrentProvince(gameController_.tickHandler.GetState());
            countryInfo_?.UpdateCurrentCountry(gameController_.tickHandler.GetState());
        }

        private void UpdateDateText()
        {
            DateButton.Text = gameController_.GetGameDateTime().ToString();
        }
    }
}
