namespace Juniper
{
    partial class SpeechGen
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.formatSelection = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.regionSelection = new System.Windows.Forms.ComboBox();
            this.genderSelection = new System.Windows.Forms.ComboBox();
            this.voiceNameSelection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.rateChange = new System.Windows.Forms.TrackBar();
            this.pitchChange = new System.Windows.Forms.TrackBar();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.playButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rateChange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchChange)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.formatSelection, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.regionSelection, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.genderSelection, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.voiceNameSelection, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.rateChange, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.pitchChange, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(25, 31);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1114, 1133);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // formatSelection
            // 
            this.formatSelection.DisplayMember = "Name";
            this.formatSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatSelection.FormattingEnabled = true;
            this.formatSelection.Location = new System.Drawing.Point(838, 55);
            this.formatSelection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.formatSelection.Name = "formatSelection";
            this.formatSelection.Size = new System.Drawing.Size(272, 33);
            this.formatSelection.TabIndex = 15;
            this.formatSelection.SelectedIndexChanged += new System.EventHandler(this.FormatSelection_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(838, 0);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(272, 50);
            this.label7.TabIndex = 14;
            this.label7.Text = "Format";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // regionSelection
            // 
            this.regionSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.regionSelection.FormattingEnabled = true;
            this.regionSelection.Location = new System.Drawing.Point(4, 55);
            this.regionSelection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.regionSelection.Name = "regionSelection";
            this.regionSelection.Size = new System.Drawing.Size(270, 33);
            this.regionSelection.TabIndex = 1;
            this.regionSelection.SelectedValueChanged += new System.EventHandler(this.RegionSelection_SelectedValueChanged);
            // 
            // genderSelection
            // 
            this.genderSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genderSelection.FormattingEnabled = true;
            this.genderSelection.Location = new System.Drawing.Point(282, 55);
            this.genderSelection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.genderSelection.Name = "genderSelection";
            this.genderSelection.Size = new System.Drawing.Size(270, 33);
            this.genderSelection.TabIndex = 2;
            this.genderSelection.SelectedValueChanged += new System.EventHandler(this.GenderSelection_SelectedValueChanged);
            // 
            // voiceNameSelection
            // 
            this.voiceNameSelection.DisplayMember = "ShortName";
            this.voiceNameSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.voiceNameSelection.FormattingEnabled = true;
            this.voiceNameSelection.Location = new System.Drawing.Point(560, 55);
            this.voiceNameSelection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.voiceNameSelection.Name = "voiceNameSelection";
            this.voiceNameSelection.Size = new System.Drawing.Size(270, 33);
            this.voiceNameSelection.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(270, 50);
            this.label1.TabIndex = 4;
            this.label1.Text = "Region";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(282, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(270, 50);
            this.label2.TabIndex = 5;
            this.label2.Text = "Gender";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(560, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(270, 50);
            this.label3.TabIndex = 6;
            this.label3.Text = "Voice";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBox
            // 
            this.textBox.AcceptsReturn = true;
            this.tableLayoutPanel1.SetColumnSpan(this.textBox, 4);
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Location = new System.Drawing.Point(4, 205);
            this.textBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(1106, 845);
            this.textBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Right;
            this.label4.Location = new System.Drawing.Point(165, 100);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 50);
            this.label4.TabIndex = 8;
            this.label4.Text = "Rate change";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Right;
            this.label5.Location = new System.Drawing.Point(162, 150);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 50);
            this.label5.TabIndex = 9;
            this.label5.Text = "Pitch change";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rateChange
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.rateChange, 3);
            this.rateChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rateChange.Location = new System.Drawing.Point(282, 105);
            this.rateChange.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rateChange.Maximum = 50;
            this.rateChange.Minimum = -50;
            this.rateChange.Name = "rateChange";
            this.rateChange.Size = new System.Drawing.Size(828, 40);
            this.rateChange.TabIndex = 10;
            // 
            // pitchChange
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.pitchChange, 3);
            this.pitchChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pitchChange.Location = new System.Drawing.Point(282, 155);
            this.pitchChange.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pitchChange.Maximum = 50;
            this.pitchChange.Minimum = -50;
            this.pitchChange.Name = "pitchChange";
            this.pitchChange.Size = new System.Drawing.Size(828, 40);
            this.pitchChange.TabIndex = 11;
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 4);
            this.flowLayoutPanel1.Controls.Add(this.playButton);
            this.flowLayoutPanel1.Controls.Add(this.saveButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 1060);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1106, 68);
            this.flowLayoutPanel1.TabIndex = 12;
            // 
            // playButton
            // 
            this.playButton.Location = new System.Drawing.Point(1008, 5);
            this.playButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(94, 50);
            this.playButton.TabIndex = 0;
            this.playButton.Text = "Play";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(906, 5);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(94, 50);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // saveFile
            // 
            this.saveFile.Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav|All files (*.*)|*.*";
            // 
            // SpeechGen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 1195);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SpeechGen";
            this.Padding = new System.Windows.Forms.Padding(25, 31, 25, 31);
            this.Text = "Azure Text to Speech";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rateChange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pitchChange)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.ComboBox regionSelection;
        private System.Windows.Forms.ComboBox genderSelection;
        private System.Windows.Forms.ComboBox voiceNameSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar rateChange;
        private System.Windows.Forms.TrackBar pitchChange;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.ComboBox formatSelection;
        private System.Windows.Forms.Label label7;
    }
}

