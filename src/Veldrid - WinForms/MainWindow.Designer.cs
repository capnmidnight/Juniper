namespace Juniper
{
    partial class MainWindow
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
            if (disposing)
            {
                veldridPanel1?.Dispose();
                veldridPanel1 = null;
                components?.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this.errorTextBox1 = new System.Windows.Forms.TextBox();
            this.veldridPanel1 = new Juniper.VeldridIntegration.WinFormsSupport.VeldridPanel();
            this.statsTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // errorTextBox1
            // 
            this.errorTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorTextBox1.Location = new System.Drawing.Point(592, 30);
            this.errorTextBox1.Multiline = true;
            this.errorTextBox1.Name = "errorTextBox1";
            this.errorTextBox1.ReadOnly = true;
            this.errorTextBox1.Size = new System.Drawing.Size(193, 424);
            this.errorTextBox1.TabIndex = 1;
            // 
            // veldridPanel1
            // 
            this.veldridPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.veldridPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.veldridPanel1.Location = new System.Drawing.Point(30, 30);
            this.veldridPanel1.Name = "veldridPanel1";
            this.veldridPanel1.Size = new System.Drawing.Size(556, 424);
            this.veldridPanel1.TabIndex = 0;
            // 
            // statsTimer
            // 
            this.statsTimer.Tick += new System.EventHandler(this.statsTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 525);
            this.Controls.Add(this.errorTextBox1);
            this.Controls.Add(this.veldridPanel1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VeldridIntegration.WinFormsSupport.VeldridPanel veldridPanel1;
        private System.Windows.Forms.TextBox errorTextBox1;
        private System.Windows.Forms.Timer statsTimer;
    }
}