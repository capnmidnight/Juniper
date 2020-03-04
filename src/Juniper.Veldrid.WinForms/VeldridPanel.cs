using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Veldrid;

namespace Juniper.VeldridIntegration.WinFormsSupport
{
    public partial class VeldridPanel : UserControl
    {
        public VeldridGraphicsDevice VeldridGraphicsDevice { get; set; }

        public event EventHandler Ready;
        public event UpdateCommandListHandler CommandListUpdate;
        private UpdateCommandListEventArgs updateArgs;

        public VeldridPanel()
        {
            var currentModule = typeof(VeldridPanel).Module;
            var hinstance = Marshal.GetHINSTANCE(currentModule);
            veldridSwapchainSource = SwapchainSource.CreateWin32(Handle, hinstance);

            InitializeComponent();
        }

        private void InitializeSwapchain()
        {
            if (VeldridSwapChain is null
                && VeldridGraphicsDevice is object)
            {
                VeldridGraphicsDevice.Prepare();
                if (VeldridGraphicsDevice.VeldridDevice is object)
                {
                    var swapchainDescription = new SwapchainDescription
                    {
                        Source = veldridSwapchainSource,
                        Width = (uint)Width,
                        Height = (uint)Height,
                        DepthFormat = (PixelFormat?)VeldridGraphicsDevice.VeldridSwapchainDepthFormat,
                        SyncToVerticalBlank = VeldridGraphicsDevice.VeldridVSync,
                        ColorSrgb = VeldridGraphicsDevice.VeldridSwapchainSRGBFormat
                    };

                    var resourceFactory = VeldridGraphicsDevice.VeldridDevice.ResourceFactory;
                    VeldridSwapChain = resourceFactory.CreateSwapchain(swapchainDescription);
                    commandList = VeldridGraphicsDevice.VeldridDevice.ResourceFactory.CreateCommandList();
                    updateArgs = new UpdateCommandListEventArgs(commandList);

                    Ready?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            VeldridSwapChain?.Resize((uint)Width, (uint)Height);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            InitializeSwapchain();

            if (commandList is object
                && updateArgs is object
                && VeldridGraphicsDevice is object
                && VeldridSwapChain is object)
            {
                CommandListUpdate?.Invoke(this, updateArgs);
                VeldridGraphicsDevice.Draw(commandList, VeldridSwapChain);
            }
        }
    }
}
