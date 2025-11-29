namespace ColdWarPrototype2
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public System.Windows.Forms.PictureBox MapPictureBox;
        public System.Windows.Forms.GroupBox MapModeGroup;
        public System.Windows.Forms.RadioButton MapModePolitical;
        public System.Windows.Forms.RadioButton MapModeRaw;
        public System.Windows.Forms.Button DateButton;
        public System.Windows.Forms.ListBox ArmyListBox;
        public System.Windows.Forms.GroupBox ProvinceBox;
        public System.Windows.Forms.Label ProvinceName;
        public System.Windows.Forms.Label ProvincePopulation;
        public System.Windows.Forms.Label ProvinceIndustrialization;
        public System.Windows.Forms.Label ProvinceEducation;
        public System.Windows.Forms.Label ProvinceProduction;
        public System.Windows.Forms.Label ProvinceTerrain;
        public System.Windows.Forms.Label ProvinceOwner;
        public System.Windows.Forms.Label ProvinceController;

        public GroupBox CountryBox;
        public System.Windows.Forms.PictureBox FlagPictureBox;
        public System.Windows.Forms.Label CountryName;
        public System.Windows.Forms.Label CountryProduction;
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
            MapPictureBox = new PictureBox();
            MapModeGroup = new GroupBox();
            MapModePolitical = new RadioButton();
            MapModeRaw = new RadioButton();
            DateButton = new Button();
            ProvinceBox = new GroupBox();
            ProvinceName = new Label();
            ProvincePopulation = new Label();
            ProvinceIndustrialization = new Label();
            ProvinceEducation = new Label();
            ProvinceProduction = new Label();
            ProvinceTerrain = new Label();
            ProvinceOwner = new Label();
            ProvinceController = new Label();
            FlagPictureBox = new PictureBox();
            CountryBox = new GroupBox();
            CountryProduction = new Label();
            CountryName = new Label();
            CountryLeader = new Label();
            CountryGovernment = new Label();
            CountryAllegiance = new Label();
            ArmyListBox = new ListBox();
            ((System.ComponentModel.ISupportInitialize)MapPictureBox).BeginInit();
            MapModeGroup.SuspendLayout();
            ProvinceBox.SuspendLayout();
            CountryBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FlagPictureBox).BeginInit();
            SuspendLayout();
            // 
            // MapPictureBox
            // 
            MapPictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MapPictureBox.Location = new Point(230, 0);
            MapPictureBox.Name = "MapPictureBox";
            MapPictureBox.Size = new Size(1654, 1000);
            MapPictureBox.TabIndex = 0;
            MapPictureBox.TabStop = false;
            // 
            // MapModeRaw
            //
            MapModeRaw.AutoSize = true;
            MapModeRaw.Checked = true;
            MapModeRaw.Location = new Point(8, 19);
            MapModeRaw.Name = "MapModeRaw";
            MapModeRaw.Size = new Size(47, 17);
            MapModeRaw.TabIndex = 2;
            MapModeRaw.TabStop = true;
            MapModeRaw.Text = "Raw";
            MapModeRaw.UseVisualStyleBackColor = true;
            // 
            // MapModePolitical
            //
            MapModePolitical.AutoSize = true;
            MapModePolitical.Location = new Point(8, 42);
            MapModePolitical.Name = "MapModePolitical";
            MapModePolitical.Size = new Size(61, 17);
            MapModePolitical.TabIndex = 1;
            MapModePolitical.Text = "Political";
            MapModeRaw.UseVisualStyleBackColor = true;
            //
            // MapModeGroup
            //
            MapModeGroup.Controls.Add(MapModePolitical);
            MapModeGroup.Controls.Add(MapModeRaw);
            MapModeGroup.Location = new Point(4, 3);
            MapModeGroup.Name = "MapModeGroup";
            MapModeGroup.Size = new Size(220, 66);
            MapModeGroup.TabIndex = 3;
            MapModeGroup.TabStop = false;
            MapModeGroup.Text = "Map Modes";
            // 
            // DateButton
            // 
            DateButton.Location = new Point(4, 952);
            DateButton.Name = "DateButton";
            DateButton.Size = new Size(216, 37);
            DateButton.TabIndex = 3;
            DateButton.Text = "DD-MM-YYYY";
            DateButton.UseVisualStyleBackColor = true;
            //
            // ProvinceBox
            //
            ProvinceBox.Controls.Add(ProvinceTerrain);
            ProvinceBox.Controls.Add(ProvinceProduction);
            ProvinceBox.Controls.Add(ProvinceEducation);
            ProvinceBox.Controls.Add(ProvinceOwner);
            ProvinceBox.Controls.Add(ProvinceController);
            ProvinceBox.Controls.Add(ProvinceIndustrialization);
            ProvinceBox.Controls.Add(ProvincePopulation);
            ProvinceBox.Controls.Add(ProvinceName);
            ProvinceBox.Location = new Point(4, 77);
            ProvinceBox.Name = "ProvinceBox";
            ProvinceBox.Size = new Size(224, 156);
            ProvinceBox.TabIndex = 4;
            ProvinceBox.TabStop = false;
            ProvinceBox.Text = "Province";
            // 
            // ProvinceName
            // 
            ProvinceName.Location = new Point(8, 18);
            ProvinceName.Name = "ProvinceName";
            ProvinceName.Size = new Size(10, 13);
            ProvinceName.TabIndex = 4;
            ProvinceName.Text = "-";
            // 
            // ProvincePopulation
            // 
            ProvincePopulation.Location = new Point(8, 38);
            ProvincePopulation.Name = "ProvincePopulation";
            ProvincePopulation.Size = new Size(13, 13);
            ProvincePopulation.TabIndex = 5;
            ProvincePopulation.Text = "0";
            // 
            // ProvinceIndustrialization
            // 
            ProvinceIndustrialization.Location = new Point(8, 57);
            ProvinceIndustrialization.Name = "ProvinceIndustrialization";
            ProvinceIndustrialization.Size = new Size(10, 13);
            ProvinceIndustrialization.TabIndex = 6;
            ProvinceIndustrialization.Text = "-";
            // 
            // ProvinceEducation
            // 
            ProvinceEducation.Location = new Point(8, 70);
            ProvinceEducation.Name = "ProvinceEducation";
            ProvinceEducation.Size = new Size(10, 13);
            ProvinceEducation.TabIndex = 7;
            ProvinceEducation.Text = "-";
            // 
            // ProvinceProduction
            // 
            ProvinceProduction.Location = new Point(8, 115);
            ProvinceProduction.Name = "ProvinceProduction";
            ProvinceProduction.Size = new Size(10, 13);
            ProvinceProduction.TabIndex = 8;
            ProvinceProduction.Text = "-";
            // 
            // ProvinceTerrain
            // 
            ProvinceTerrain.Location = new Point(8, 128);
            ProvinceTerrain.Name = "ProvinceTerrain";
            ProvinceTerrain.Size = new Size(10, 13);
            ProvinceTerrain.TabIndex = 9;
            ProvinceTerrain.Text = "-";
            // 
            // ProvinceOwner
            // 
            ProvinceOwner.Location = new Point(8, 83);
            ProvinceOwner.Name = "ProvinceOwner";
            ProvinceOwner.Size = new Size(10, 13);
            ProvinceOwner.TabIndex = 10;
            ProvinceOwner.Text = "-";
            // 
            // ProvinceController
            // 
            ProvinceController.Location = new Point(8, 96);
            ProvinceController.Name = "ProvinceController";
            ProvinceController.Size = new Size(10, 13);
            ProvinceController.TabIndex = 11;
            ProvinceController.Text = "-";
            //
            // CountryBox
            //
            CountryBox.Controls.Add(FlagPictureBox);
            CountryBox.Controls.Add(CountryProduction);
            CountryBox.Controls.Add(CountryAllegiance);
            CountryBox.Controls.Add(CountryGovernment);
            CountryBox.Controls.Add(CountryLeader);
            CountryBox.Controls.Add(CountryName);
            CountryBox.Location = new Point(4, 239);
            CountryBox.Name = "CountryBox";
            CountryBox.Size = new Size(224, 93);
            CountryBox.TabIndex = 5;
            CountryBox.TabStop = false;
            CountryBox.Text = "Country";
            //
            // FlagPictureBox
            // 
            FlagPictureBox.Location = new Point(174, 16);
            FlagPictureBox.Name = "FlagPictureBox";
            FlagPictureBox.Size = new Size(45, 30);
            FlagPictureBox.TabIndex = 12;
            FlagPictureBox.TabStop = false;
            // 
            // CountryName
            // 
            CountryName.Location = new Point(8, 16);
            CountryName.Name = "CountryName";
            CountryName.Size = new Size(10, 13);
            CountryName.TabIndex = 13;
            CountryName.Text = "-";
            // 
            // CountryLeader
            // 
            CountryLeader.Location = new Point(8, 29);
            CountryLeader.Name = "CountryLeader";
            CountryLeader.Size = new Size(10, 13);
            CountryLeader.TabIndex = 14;
            CountryLeader.Text = "";
            // 
            // CountryGovernment
            // 
            CountryGovernment.Location = new Point(8, 42);
            CountryGovernment.Name = "CountryGovernment";
            CountryGovernment.Size = new Size(10, 13);
            CountryGovernment.TabIndex = 15;
            CountryGovernment.Text = "";
            // 
            // CountryAllegiance
            // 
            CountryAllegiance.Location = new Point(8, 55);
            CountryAllegiance.Name = "CountryAllegiance";
            CountryAllegiance.Size = new Size(10, 13);
            CountryAllegiance.TabIndex = 16;
            CountryAllegiance.Text = "";
            // 
            // ArmyListBox
            // 
            ArmyListBox.Location = new Point(4, 341);
            ArmyListBox.Name = "ArmyListBox";
            ArmyListBox.Size = new Size(223, 199);
            ArmyListBox.TabIndex = 17;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1884, 1001);
            Controls.Add(MapPictureBox);
            Controls.Add(MapModeGroup);
            Controls.Add(DateButton);
            Controls.Add(ArmyListBox);
            Controls.Add(ProvinceBox);
            Controls.Add(CountryBox);
            //Controls.Add(CoordinateDisplay);
            //Controls.Add(MoveArmiesButton);
            Name = "MainWindow";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Cold War Strategy Prototype";
            ((System.ComponentModel.ISupportInitialize)MapPictureBox).EndInit();
            MapModeGroup.ResumeLayout(false);
            MapModeGroup.PerformLayout();
            ProvinceBox.ResumeLayout(false);
            ProvinceBox.PerformLayout();
            CountryBox.ResumeLayout(false);
            CountryBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)FlagPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
