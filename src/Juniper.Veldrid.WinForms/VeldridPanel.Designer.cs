using Veldrid;

namespace Juniper.VeldridIntegration.WinFormsSupport
{
    partial class VeldridPanel
    {
        private readonly SwapchainSource veldridSwapchainSource;
        private CommandList commandList;
        public Swapchain VeldridSwapChain { get; private set; }


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
                VeldridSwapChain?.Dispose();
                VeldridSwapChain = null;
                commandList?.Dispose();
                commandList = null;
                components?.Dispose();
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
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }

        #endregion
    }
}
