namespace ColdWarPrototype.Dialogs
{
    partial class EventDialog
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            EventPictureBox = new PictureBox();
            EventTextBox = new RichTextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            ((System.ComponentModel.ISupportInitialize)EventPictureBox).BeginInit();
            SuspendLayout();
            // 
            // EventPictureBox
            // 
            EventPictureBox.Location = new Point(3, 3);
            EventPictureBox.Name = "EventPictureBox";
            EventPictureBox.Size = new Size(768, 512);
            EventPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            EventPictureBox.TabIndex = 0;
            EventPictureBox.TabStop = false;
            // 
            // EventTextBox
            // 
            EventTextBox.Location = new Point(776, 4);
            EventTextBox.Name = "EventTextBox";
            EventTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            EventTextBox.Size = new Size(725, 511);
            EventTextBox.TabIndex = 1;
            EventTextBox.Text = "";
            EventTextBox.WordWrap = false;
            // 
            // button1
            // 
            button1.Location = new Point(527, 521);
            button1.Name = "button1";
            button1.Size = new Size(502, 32);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += OptionButton_Click;
            // 
            // button2
            // 
            button2.Location = new Point(527, 559);
            button2.Name = "button2";
            button2.Size = new Size(502, 32);
            button2.TabIndex = 3;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(527, 597);
            button3.Name = "button3";
            button3.Size = new Size(502, 32);
            button3.TabIndex = 4;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // EventDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(EventTextBox);
            Controls.Add(EventPictureBox);
            Name = "EventDialog";
            Size = new Size(1504, 644);
            ((System.ComponentModel.ISupportInitialize)EventPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox EventPictureBox;
        private RichTextBox EventTextBox;
        private Button button1;
        private Button button2;
        private Button button3;
    }
}
