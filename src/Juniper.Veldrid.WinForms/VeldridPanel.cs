using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Veldrid;

namespace Juniper.VeldridIntegration.WinFormsSupport
{
    public partial class VeldridPanel : UserControl
    {
        public VeldridGraphicsDevice VeldridGraphicsDevice { get; set; }

        public event UpdateCommandListHandler CommandListUpdate;
        private UpdateCommandListEventArgs updateArgs;

        private void OnUpdateCommandList()
        {
            if (commandList is null
                && VeldridGraphicsDevice is object
                && VeldridGraphicsDevice.VeldridDevice is object)
            {
                commandList = VeldridGraphicsDevice.VeldridDevice.ResourceFactory.CreateCommandList();
                updateArgs = new UpdateCommandListEventArgs(commandList);
            }

            if (commandList is object)
            {
                CommandListUpdate?.Invoke(this, updateArgs);
            }
        }

        public VeldridPanel()
        {
            var currentModule = typeof(VeldridPanel).Module;
            var hinstance = Marshal.GetHINSTANCE(currentModule);
            veldridSwapchainSource = SwapchainSource.CreateWin32(Handle, hinstance);

            InitializeComponent();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            VeldridSwapChain?.Resize((uint)Width, (uint)Height);
            Invalidate();
        }

        private bool prepared;
        public void Prepare()
        {
            if (VeldridGraphicsDevice != null
                && !prepared)
            {
                VeldridGraphicsDevice.Prepare();

                var swapchainDescription = new SwapchainDescription
                {
                    Source = veldridSwapchainSource,
                    Width = (uint)Width,
                    Height = (uint)Height,
                    DepthFormat = (PixelFormat?)VeldridGraphicsDevice.VeldridSwapchainDepthFormat,
                    SyncToVerticalBlank = VeldridGraphicsDevice.VeldridVSync,
                    ColorSrgb = VeldridGraphicsDevice.VeldridSwapchainSRGBFormat
                };

                var resourceFactory = VeldridGraphicsDevice.ResourceFactory;
                VeldridSwapChain = resourceFactory.CreateSwapchain(swapchainDescription);

                prepared = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (VeldridGraphicsDevice != null
                && VeldridSwapChain != null)
            {
                OnUpdateCommandList();
                VeldridGraphicsDevice.Draw(commandList, VeldridSwapChain);
            }
        }
    }
}
