using Veldrid;

namespace Juniper.VeldridIntegration.WinFormsSupport
{
    partial class VeldridGraphicsDevice
    {
        
        public GraphicsBackend Backend { get; set; }

        public GraphicsDeviceOptions veldridDeviceOptions;

        public GraphicsDeviceOptions Options => veldridDeviceOptions;

        public bool VeldridHasMainSwapchain
        {
            get => veldridDeviceOptions.HasMainSwapchain;
            set => veldridDeviceOptions.HasMainSwapchain = value;
        }

        public bool VeldridPreferDepthRangeZeroToOne
        {
            get => veldridDeviceOptions.PreferDepthRangeZeroToOne;
            set => veldridDeviceOptions.PreferDepthRangeZeroToOne = value;
        }

        public ResourceBindingModel VeldridResourceBindingModel
        {
            get => veldridDeviceOptions.ResourceBindingModel;
            set => veldridDeviceOptions.ResourceBindingModel = value;
        }

        public SwapchainDepthFormat? VeldridSwapchainDepthFormat
        {
            get => (SwapchainDepthFormat?)veldridDeviceOptions.SwapchainDepthFormat;
            set => veldridDeviceOptions.SwapchainDepthFormat = (PixelFormat?)value;
        }

        public bool VeldridSwapchainSRGBFormat
        {
            get => veldridDeviceOptions.SwapchainSrgbFormat;
            set => veldridDeviceOptions.SwapchainSrgbFormat = value;
        }

        public bool VeldridVSync
        {
            get => veldridDeviceOptions.SyncToVerticalBlank;
            set => veldridDeviceOptions.SyncToVerticalBlank = value;
        }

        public bool VeldridPreferStandardClipSpaceYDirection
        {
            get => veldridDeviceOptions.PreferStandardClipSpaceYDirection;
            set => veldridDeviceOptions.PreferStandardClipSpaceYDirection = value;
        }

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
        }

        #endregion
    }
}
