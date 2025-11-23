namespace ColdWarPrototype2
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public System.Windows.Forms.PictureBox MapPictureBox;
        public System.Windows.Forms.RadioButton MapModePolitical;
        public System.Windows.Forms.RadioButton MapModeRaw;
        public System.Windows.Forms.Button DateButton;
        public System.Windows.Forms.ListBox ArmyListBox;
        public System.Windows.Forms.Label ProvinceName;
        public System.Windows.Forms.Label ProvincePopulation;
        public System.Windows.Forms.Label ProvinceIndustrialization;
        public System.Windows.Forms.Label ProvinceEducation;
        public System.Windows.Forms.Label ProvinceProduction;
        public System.Windows.Forms.Label ProvinceTerrain;
        public System.Windows.Forms.Label ProvinceOwner;
        public System.Windows.Forms.Label ProvinceController;

        public System.Windows.Forms.PictureBox FlagPictureBox;
        public System.Windows.Forms.Label CountryName;
        public System.Windows.Forms.Label CountryLeader;
        public System.Windows.Forms.Label CountryGovernment;
        public System.Windows.Forms.Label CountryAllegiance;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 768);
            Text = "Cold War Prototype 2";

            // Controls migrated from original UI
            MapPictureBox = new PictureBox { Location = new Point(230, 0), Size = new Size(1654, 1000) };
            MapPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            MapPictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            MapModePolitical = new RadioButton { Location = new Point(10, 10), Text = "Political", Checked = true };
            MapModeRaw = new RadioButton { Location = new Point(120, 10), Text = "Raw" };
            DateButton = new Button { Location = new Point(240, 10), Text = "Date" };

            // Province info labels (placed to the right of the map)
            ProvinceName = new Label { Location = new Point(820, 10), Size = new Size(180, 20), Text = "" };
            ProvincePopulation = new Label { Location = new Point(820, 30), Size = new Size(180, 20), Text = "" };
            ProvinceIndustrialization = new Label { Location = new Point(820, 50), Size = new Size(180, 20), Text = "" };
            ProvinceEducation = new Label { Location = new Point(820, 70), Size = new Size(180, 20), Text = "" };
            ProvinceProduction = new Label { Location = new Point(820, 90), Size = new Size(180, 20), Text = "" };
            ProvinceTerrain = new Label { Location = new Point(820, 110), Size = new Size(180, 20), Text = "" };
            ProvinceOwner = new Label { Location = new Point(820, 130), Size = new Size(180, 20), Text = "" };
            ProvinceController = new Label { Location = new Point(820, 150), Size = new Size(180, 20), Text = "" };

            // Country info
            FlagPictureBox = new PictureBox { Location = new Point(820, 180), Size = new Size(64, 40) };
            CountryName = new Label { Location = new Point(890, 180), Size = new Size(110, 20), Text = "" };
            CountryLeader = new Label { Location = new Point(890, 200), Size = new Size(110, 20), Text = "" };
            CountryGovernment = new Label { Location = new Point(890, 220), Size = new Size(110, 20), Text = "" };
            CountryAllegiance = new Label { Location = new Point(890, 240), Size = new Size(110, 20), Text = "" };

            ArmyListBox = new ListBox { Location = new Point(820, 260), Size = new Size(180, 380) };

            Controls.Add(MapPictureBox);
            Controls.Add(MapModePolitical);
            Controls.Add(MapModeRaw);
            Controls.Add(DateButton);
            Controls.Add(ProvinceName);
            Controls.Add(ProvincePopulation);
            Controls.Add(ProvinceIndustrialization);
            Controls.Add(ProvinceEducation);
            Controls.Add(ProvinceProduction);
            Controls.Add(ProvinceTerrain);
            Controls.Add(ProvinceOwner);
            Controls.Add(ProvinceController);
            Controls.Add(FlagPictureBox);
            Controls.Add(CountryName);
            Controls.Add(CountryLeader);
            Controls.Add(CountryGovernment);
            Controls.Add(CountryAllegiance);
            Controls.Add(ArmyListBox);
        }

        #endregion
    }
}
