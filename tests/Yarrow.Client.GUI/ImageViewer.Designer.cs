namespace Yarrow.Client.GUI
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
            ((System.ComponentModel.ISupportInitialize)(this.cubeMapPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // cubeMapPictureBox
            // 
            this.cubeMapPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cubeMapPictureBox.Location = new System.Drawing.Point(0, 0);
            this.cubeMapPictureBox.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.cubeMapPictureBox.Name = "cubeMapPictureBox";
            this.cubeMapPictureBox.Size = new System.Drawing.Size(550, 535);
            this.cubeMapPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cubeMapPictureBox.TabIndex = 0;
            this.cubeMapPictureBox.TabStop = false;
            // 
            // ImageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 535);
            this.Controls.Add(this.cubeMapPictureBox);
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.Name = "ImageViewer";
            this.Text = "Image Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.cubeMapPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox cubeMapPictureBox;
    }
}

