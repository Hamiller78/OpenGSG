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
            EventPictureBox.Location = new Point(5, 5);
            EventPictureBox.Margin = new Padding(5);
            EventPictureBox.Name = "EventPictureBox";
            EventPictureBox.Size = new Size(768, 512);
            EventPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            EventPictureBox.TabIndex = 0;
            EventPictureBox.TabStop = false;
            // 
            // EventTextBox
            // 
            EventTextBox.Location = new Point(783, 5);
            EventTextBox.Margin = new Padding(5);
            EventTextBox.Name = "EventTextBox";
            EventTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            EventTextBox.Size = new Size(800, 512);
            EventTextBox.TabIndex = 1;
            EventTextBox.Text = "";
            // 
            // button1
            // 
            button1.Location = new Point(369, 527);
            button1.Margin = new Padding(5);
            button1.Name = "button1";
            button1.Size = new Size(789, 53);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(369, 590);
            button2.Margin = new Padding(5);
            button2.Name = "button2";
            button2.Size = new Size(789, 53);
            button2.TabIndex = 3;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(369, 653);
            button3.Margin = new Padding(5);
            button3.Name = "button3";
            button3.Size = new Size(789, 53);
            button3.TabIndex = 4;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // EventDialog
            // 
            AutoScaleDimensions = new SizeF(11F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1598, 718);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(EventTextBox);
            Controls.Add(EventPictureBox);
            Font = new Font("Segoe UI", 14F);
            Margin = new Padding(5);
            Name = "EventDialog";
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
