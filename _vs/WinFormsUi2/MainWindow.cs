using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenGSGLibrary.Tools;
using Simulation;
using WorldData;
using WinFormsUi2.Views;

namespace WinFormsUi2
{
    public class MainWindow : Form
    {
        public Logger log = new Logger("CWPLog.log", Directory.GetCurrentDirectory());
        private MasterController gameController_ = new MasterController();

        // Views
        private WorldMapView worldMapView_;

        // Controllers
        private Gui.MouseController mouseController_;

        // UI controls
        public PictureBox MapPictureBox { get; } = new PictureBox { Dock = DockStyle.Fill };
        public RadioButton MapModePolitical { get; } = new RadioButton { Text = "Political", Checked = true };
        public RadioButton MapModeRaw { get; } = new RadioButton { Text = "Raw" };
        public Button DateButton { get; } = new Button();

        public MainWindow()
        {
            Text = "WinFormsUi2 - Migrated";
            Width = 1024; Height = 768;

            var panel = new Panel { Dock = DockStyle.Top, Height = 30 };
            panel.Controls.Add(MapModePolitical);
            panel.Controls.Add(MapModeRaw);
            panel.Controls.Add(DateButton);
            Controls.Add(panel);
            Controls.Add(MapPictureBox);

            Load += MainWindow_Load;

            worldMapView_ = new WorldMapView(this);
        }

        private void MainWindow_Load(object? sender, EventArgs e)
        {
            log.WriteLine(LogLevel.Info, "Session started");
            gameController_.Init();

            worldMapView_.SetSourceProvinceMap(gameController_.GetWorldManager().provinceMap);
            worldMapView_.UpdateCountryMap(gameController_.tickHandler.GetState());

            mouseController_ = new Gui.MouseController(gameController_);
            var sourceMapSize = gameController_.worldData.provinceMap.sourceBitmap!.Size;
            mouseController_.SetMapScalingFactor(MapPictureBox.Size, sourceMapSize);

            mouseController_.HoveredProvinceChanged += (s, ev) => { /* TODO wire to view */ };
            mouseController_.HoveredCountryChanged += (s, ev) => { /* TODO wire to view */ };

            MapPictureBox.MouseMove += (s, ev) => mouseController_.HandleMouseMovedOverMap(ev);
            MapModePolitical.CheckedChanged += (s, ev) => worldMapView_.SetMapPicture();
            MapModeRaw.CheckedChanged += (s, ev) => worldMapView_.SetMapPicture();

            DateButton.Click += (s, ev) =>
            {
                gameController_.tickHandler.FinishTick();
                DateButton.Text = gameController_.GetGameDateTime().ToString();
            };

            DateButton.Text = gameController_.GetGameDateTime().ToString();
        }
    }
}
