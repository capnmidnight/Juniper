namespace Yarrow.Client.GUI.WinForms
{
    partial class ImageViewer
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
            this.cubeMapPictureBox = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.locationTextBox = new System.Windows.Forms.TextBox();
            this.submitButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cubeMapPictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cubeMapPictureBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.cubeMapPictureBox, 2);
            this.cubeMapPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cubeMapPictureBox.Location = new System.Drawing.Point(3, 2);
            this.cubeMapPictureBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cubeMapPictureBox.Name = "cubeMapPictureBox";
            this.cubeMapPictureBox.Size = new System.Drawing.Size(1679, 1477);
            this.cubeMapPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cubeMapPictureBox.TabIndex = 0;
            this.cubeMapPictureBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.94936F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.05063F));
            this.tableLayoutPanel1.Controls.Add(this.cubeMapPictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.locationTextBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.submitButton, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95.68567F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.31433F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1685, 1548);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // locationTextBox
            // 
            this.locationTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locationTextBox.Location = new System.Drawing.Point(8, 1488);
            this.locationTextBox.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(1263, 38);
            this.locationTextBox.TabIndex = 1;
            this.locationTextBox.Text = "Alexandria, VA";
            this.locationTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LocationTextBox_KeyUp);
            // 
            // submitButton
            // 
            this.submitButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.submitButton.Location = new System.Drawing.Point(1287, 1488);
            this.submitButton.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(390, 53);
            this.submitButton.TabIndex = 2;
            this.submitButton.Text = "&Submit";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // ImageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1685, 1548);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ImageViewer";
            this.Text = "Image Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.cubeMapPictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox cubeMapPictureBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox locationTextBox;
        private System.Windows.Forms.Button submitButton;
    }
}

