using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ColdWarPrototype2.Views;
using Simulation;
using Tools;

namespace ColdWarPrototype2
{
    public partial class MainWindow : Form
    {
        private Logger log;
        private MasterController gameController_ = new MasterController();

        // view helpers
        private Views.ProvinceInfo? provinceInfo_;
        private Views.CountryInfo? countryInfo_;
        private Views.ArmyList? armyListView_;

        public MainWindow()
        {
            InitializeComponent();
            log = new Logger("CWPLog.log", Directory.GetCurrentDirectory());
            Load += Form1_Load;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            log.WriteLine(LogLevel.Info, "Session started (migrated UI)");
            try
            {
                gameController_.Init();

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
                armyListView_ = new Views.ArmyList(this);

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
                            countryInfo_?.UpdateCurrentCountry(state, owner);
                            armyListView_?.FillListBox(ArmyListBox, state, provinceId);
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

                var worldMapView = new Views.WorldMapView(this);
                worldMapView.SetSourceProvinceMap(provinceMap);
                worldMapView.UpdateCountryMap(gameController_.tickHandler.GetState());
            }
            catch (Exception ex)
            {
                log.WriteLine(LogLevel.Err, "Error initializing game controller: " + ex.Message);
            }
        }

        private void DateButton_Click(object? sender, EventArgs e)
        {
            gameController_.tickHandler.FinishTick();
            DateButton.Text = gameController_.GetGameDateTime().ToString();
        }
    }
}
