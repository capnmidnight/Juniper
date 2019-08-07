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
            this.panoTextbox = new System.Windows.Forms.TextBox();
            this.addressLabel = new System.Windows.Forms.Label();
            this.panoLabel = new System.Windows.Forms.Label();
            this.latLngLabel = new System.Windows.Forms.Label();
            this.latLngTextbox = new System.Windows.Forms.TextBox();
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
            this.cubeMapPictureBox.Size = new System.Drawing.Size(985, 914);
            this.cubeMapPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cubeMapPictureBox.TabIndex = 0;
            this.cubeMapPictureBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.2553F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.74471F));
            this.tableLayoutPanel1.Controls.Add(this.cubeMapPictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.locationTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panoTextbox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.addressLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panoLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.latLngLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.latLngTextbox, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.96111F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.038895F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(991, 1091);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // locationTextBox
            // 
            this.locationTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locationTextBox.Location = new System.Drawing.Point(174, 921);
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.Size = new System.Drawing.Size(814, 38);
            this.locationTextBox.TabIndex = 1;
            this.locationTextBox.Text = "Alexandria, VA";
            this.locationTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LocationTextBox_KeyUp);
            // 
            // panoTextbox
            // 
            this.panoTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panoTextbox.Location = new System.Drawing.Point(174, 980);
            this.panoTextbox.Name = "panoTextbox";
            this.panoTextbox.Size = new System.Drawing.Size(814, 38);
            this.panoTextbox.TabIndex = 3;
            this.panoTextbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PanoTextbox_KeyUp);
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.addressLabel.Location = new System.Drawing.Point(49, 918);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(119, 59);
            this.addressLabel.TabIndex = 4;
            this.addressLabel.Text = "Address";
            // 
            // panoLabel
            // 
            this.panoLabel.AutoSize = true;
            this.panoLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.panoLabel.Location = new System.Drawing.Point(86, 977);
            this.panoLabel.Name = "panoLabel";
            this.panoLabel.Size = new System.Drawing.Size(82, 57);
            this.panoLabel.TabIndex = 5;
            this.panoLabel.Text = "Pano";
            // 
            // latLngLabel
            // 
            this.latLngLabel.AutoSize = true;
            this.latLngLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.latLngLabel.Location = new System.Drawing.Point(57, 1034);
            this.latLngLabel.Name = "latLngLabel";
            this.latLngLabel.Size = new System.Drawing.Size(111, 57);
            this.latLngLabel.TabIndex = 6;
            this.latLngLabel.Text = "Lat/Lng";
            // 
            // latLngTextbox
            // 
            this.latLngTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.latLngTextbox.Location = new System.Drawing.Point(174, 1037);
            this.latLngTextbox.Name = "latLngTextbox";
            this.latLngTextbox.Size = new System.Drawing.Size(814, 38);
            this.latLngTextbox.TabIndex = 7;
            this.latLngTextbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LatLngTextbox_KeyUp);
            // 
            // ImageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1011, 1111);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ImageViewer";
            this.Padding = new System.Windows.Forms.Padding(10);
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
        private System.Windows.Forms.TextBox panoTextbox;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.Label panoLabel;
        private System.Windows.Forms.Label latLngLabel;
        private System.Windows.Forms.TextBox latLngTextbox;
    }
}

