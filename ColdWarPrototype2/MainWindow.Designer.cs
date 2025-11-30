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
            ProvinceTerrain = new Label();
            ProvinceProduction = new Label();
            ProvinceEducation = new Label();
            ProvinceOwner = new Label();
            ProvinceController = new Label();
            ProvinceIndustrialization = new Label();
            ProvincePopulation = new Label();
            ProvinceName = new Label();
            FlagPictureBox = new PictureBox();
            CountryBox = new GroupBox();
            CountryProduction = new Label();
            CountryAllegiance = new Label();
            CountryGovernment = new Label();
            CountryLeader = new Label();
            CountryName = new Label();
            ArmyListBox = new ListBox();
            ((System.ComponentModel.ISupportInitialize)MapPictureBox).BeginInit();
            MapModeGroup.SuspendLayout();
            ProvinceBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FlagPictureBox).BeginInit();
            CountryBox.SuspendLayout();
            SuspendLayout();
            // 
            // MapPictureBox
            // 
            MapPictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MapPictureBox.Location = new Point(268, 0);
            MapPictureBox.Margin = new Padding(4, 3, 4, 3);
            MapPictureBox.Name = "MapPictureBox";
            MapPictureBox.Size = new Size(1930, 1154);
            MapPictureBox.TabIndex = 0;
            MapPictureBox.TabStop = false;
            // 
            // MapModeGroup
            // 
            MapModeGroup.Controls.Add(MapModePolitical);
            MapModeGroup.Controls.Add(MapModeRaw);
            MapModeGroup.Location = new Point(5, 3);
            MapModeGroup.Margin = new Padding(4, 3, 4, 3);
            MapModeGroup.Name = "MapModeGroup";
            MapModeGroup.Padding = new Padding(4, 3, 4, 3);
            MapModeGroup.Size = new Size(257, 76);
            MapModeGroup.TabIndex = 3;
            MapModeGroup.TabStop = false;
            MapModeGroup.Text = "Map Modes";
            // 
            // MapModePolitical
            // 
            MapModePolitical.AutoSize = true;
            MapModePolitical.Location = new Point(9, 48);
            MapModePolitical.Margin = new Padding(4, 3, 4, 3);
            MapModePolitical.Name = "MapModePolitical";
            MapModePolitical.Size = new Size(67, 19);
            MapModePolitical.TabIndex = 1;
            MapModePolitical.Text = "Political";
            // 
            // MapModeRaw
            // 
            MapModeRaw.AutoSize = true;
            MapModeRaw.Checked = true;
            MapModeRaw.Location = new Point(9, 22);
            MapModeRaw.Margin = new Padding(4, 3, 4, 3);
            MapModeRaw.Name = "MapModeRaw";
            MapModeRaw.Size = new Size(47, 19);
            MapModeRaw.TabIndex = 2;
            MapModeRaw.TabStop = true;
            MapModeRaw.Text = "Raw";
            MapModeRaw.UseVisualStyleBackColor = true;
            // 
            // DateButton
            // 
            DateButton.Location = new Point(5, 1098);
            DateButton.Margin = new Padding(4, 3, 4, 3);
            DateButton.Name = "DateButton";
            DateButton.Size = new Size(252, 43);
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
            ProvinceBox.Location = new Point(5, 89);
            ProvinceBox.Margin = new Padding(4, 3, 4, 3);
            ProvinceBox.Name = "ProvinceBox";
            ProvinceBox.Padding = new Padding(4, 3, 4, 3);
            ProvinceBox.Size = new Size(261, 180);
            ProvinceBox.TabIndex = 4;
            ProvinceBox.TabStop = false;
            ProvinceBox.Text = "Province";
            // 
            // ProvinceTerrain
            // 
            ProvinceTerrain.Location = new Point(9, 148);
            ProvinceTerrain.Margin = new Padding(4, 0, 4, 0);
            ProvinceTerrain.Name = "ProvinceTerrain";
            ProvinceTerrain.Size = new Size(233, 15);
            ProvinceTerrain.TabIndex = 9;
            ProvinceTerrain.Text = "-";
            // 
            // ProvinceProduction
            // 
            ProvinceProduction.Location = new Point(9, 133);
            ProvinceProduction.Margin = new Padding(4, 0, 4, 0);
            ProvinceProduction.Name = "ProvinceProduction";
            ProvinceProduction.Size = new Size(233, 15);
            ProvinceProduction.TabIndex = 8;
            ProvinceProduction.Text = "-";
            // 
            // ProvinceEducation
            // 
            ProvinceEducation.Location = new Point(9, 81);
            ProvinceEducation.Margin = new Padding(4, 0, 4, 0);
            ProvinceEducation.Name = "ProvinceEducation";
            ProvinceEducation.Size = new Size(233, 15);
            ProvinceEducation.TabIndex = 7;
            ProvinceEducation.Text = "-";
            // 
            // ProvinceOwner
            // 
            ProvinceOwner.Location = new Point(9, 96);
            ProvinceOwner.Margin = new Padding(4, 0, 4, 0);
            ProvinceOwner.Name = "ProvinceOwner";
            ProvinceOwner.Size = new Size(233, 15);
            ProvinceOwner.TabIndex = 10;
            ProvinceOwner.Text = "-";
            // 
            // ProvinceController
            // 
            ProvinceController.Location = new Point(9, 111);
            ProvinceController.Margin = new Padding(4, 0, 4, 0);
            ProvinceController.Name = "ProvinceController";
            ProvinceController.Size = new Size(233, 15);
            ProvinceController.TabIndex = 11;
            ProvinceController.Text = "-";
            // 
            // ProvinceIndustrialization
            // 
            ProvinceIndustrialization.Location = new Point(9, 66);
            ProvinceIndustrialization.Margin = new Padding(4, 0, 4, 0);
            ProvinceIndustrialization.Name = "ProvinceIndustrialization";
            ProvinceIndustrialization.Size = new Size(233, 15);
            ProvinceIndustrialization.TabIndex = 6;
            ProvinceIndustrialization.Text = "-";
            // 
            // ProvincePopulation
            // 
            ProvincePopulation.Location = new Point(9, 44);
            ProvincePopulation.Margin = new Padding(4, 0, 4, 0);
            ProvincePopulation.Name = "ProvincePopulation";
            ProvincePopulation.Size = new Size(233, 15);
            ProvincePopulation.TabIndex = 5;
            ProvincePopulation.Text = "0";
            // 
            // ProvinceName
            // 
            ProvinceName.Location = new Point(9, 21);
            ProvinceName.Margin = new Padding(4, 0, 4, 0);
            ProvinceName.Name = "ProvinceName";
            ProvinceName.Size = new Size(233, 15);
            ProvinceName.TabIndex = 4;
            ProvinceName.Text = "-";
            // 
            // FlagPictureBox
            // 
            FlagPictureBox.Location = new Point(203, 18);
            FlagPictureBox.Margin = new Padding(4, 3, 4, 3);
            FlagPictureBox.Name = "FlagPictureBox";
            FlagPictureBox.Size = new Size(52, 35);
            FlagPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            FlagPictureBox.TabIndex = 12;
            FlagPictureBox.TabStop = false;
            // 
            // CountryBox
            // 
            CountryBox.Controls.Add(FlagPictureBox);
            CountryBox.Controls.Add(CountryProduction);
            CountryBox.Controls.Add(CountryAllegiance);
            CountryBox.Controls.Add(CountryGovernment);
            CountryBox.Controls.Add(CountryLeader);
            CountryBox.Controls.Add(CountryName);
            CountryBox.Location = new Point(5, 276);
            CountryBox.Margin = new Padding(4, 3, 4, 3);
            CountryBox.Name = "CountryBox";
            CountryBox.Padding = new Padding(4, 3, 4, 3);
            CountryBox.Size = new Size(261, 107);
            CountryBox.TabIndex = 5;
            CountryBox.TabStop = false;
            CountryBox.Text = "Country";
            // 
            // CountryProduction
            // 
            CountryProduction.Location = new Point(9, 78);
            CountryProduction.Margin = new Padding(4, 0, 4, 0);
            CountryProduction.Name = "CountryProduction";
            CountryProduction.Size = new Size(187, 15);
            CountryProduction.TabIndex = 13;
            // 
            // CountryAllegiance
            // 
            CountryAllegiance.Location = new Point(9, 63);
            CountryAllegiance.Margin = new Padding(4, 0, 4, 0);
            CountryAllegiance.Name = "CountryAllegiance";
            CountryAllegiance.Size = new Size(187, 15);
            CountryAllegiance.TabIndex = 16;
            // 
            // CountryGovernment
            // 
            CountryGovernment.Location = new Point(9, 48);
            CountryGovernment.Margin = new Padding(4, 0, 4, 0);
            CountryGovernment.Name = "CountryGovernment";
            CountryGovernment.Size = new Size(187, 15);
            CountryGovernment.TabIndex = 15;
            // 
            // CountryLeader
            // 
            CountryLeader.Location = new Point(9, 33);
            CountryLeader.Margin = new Padding(4, 0, 4, 0);
            CountryLeader.Name = "CountryLeader";
            CountryLeader.Size = new Size(187, 15);
            CountryLeader.TabIndex = 14;
            // 
            // CountryName
            // 
            CountryName.Location = new Point(9, 18);
            CountryName.Margin = new Padding(4, 0, 4, 0);
            CountryName.Name = "CountryName";
            CountryName.Size = new Size(187, 15);
            CountryName.TabIndex = 13;
            CountryName.Text = "-";
            // 
            // ArmyListBox
            // 
            ArmyListBox.Location = new Point(5, 393);
            ArmyListBox.Margin = new Padding(4, 3, 4, 3);
            ArmyListBox.Name = "ArmyListBox";
            ArmyListBox.Size = new Size(259, 229);
            ArmyListBox.TabIndex = 17;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1924, 1061);
            Controls.Add(MapPictureBox);
            Controls.Add(MapModeGroup);
            Controls.Add(DateButton);
            Controls.Add(ArmyListBox);
            Controls.Add(ProvinceBox);
            Controls.Add(CountryBox);
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cold War Strategy Prototype";
            ((System.ComponentModel.ISupportInitialize)MapPictureBox).EndInit();
            MapModeGroup.ResumeLayout(false);
            MapModeGroup.PerformLayout();
            ProvinceBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)FlagPictureBox).EndInit();
            CountryBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}
