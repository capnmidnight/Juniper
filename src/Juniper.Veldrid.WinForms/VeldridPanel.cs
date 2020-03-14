using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Veldrid;

namespace Juniper.VeldridIntegration.WinFormsSupport
{
    public partial class VeldridPanel : UserControl
    {
        public VeldridGraphicsDevice VeldridGraphicsDevice { get; set; }

        public event EventHandler Ready;
        public event UpdateCommandListHandler CommandListUpdate;

        private bool render;
        private UpdateCommandListEventArgs updateArgs;



        public VeldridPanel()
        {
            InitializeComponent();

            Paint += VeldridPanel_Paint;
            Resize += VeldridPanel_Resize;

            var currentModule = typeof(VeldridPanel).Module;
            var hinstance = Marshal.GetHINSTANCE(currentModule);
            veldridSwapchainSource = SwapchainSource.CreateWin32(Handle, hinstance);
            canceller = new CancellationTokenSource();
        }

        private void VeldridPanel_Resize(object sender, EventArgs e)
        {
            render = false;
            VeldridSwapChain?.Resize((uint)Width, (uint)Height);
            InitializeSwapchain();
            render = true;
        }

        private void VeldridPanel_Paint(object sender, PaintEventArgs e)
        {
            using var g = CreateGraphics();
            g.FillRectangle(SystemBrushes.ControlDark, ClientRectangle);

            var icon = SystemIcons.Application;
            var middleRect = ClientRectangle;
            middleRect.X = (middleRect.Width - icon.Width * 2) / 2;
            middleRect.Y = (middleRect.Height - icon.Height * 2) / 2;
            middleRect.Width = icon.Width * 2;
            middleRect.Height = icon.Height * 2;
            g.DrawIcon(icon, middleRect);

            InitializeSwapchain();
            Invalidate();
        }

        private void RenderThread()
        {
            while (!canceller.IsCancellationRequested)
            {
                if (render)
                {
                    CommandListUpdate(this, updateArgs);
                    VeldridGraphicsDevice.Draw(commandList, VeldridSwapChain);
                }
            }
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
                    Paint -= VeldridPanel_Paint;
                    Ready?.Invoke(this, EventArgs.Empty);
                    renderThread = Task.Factory.StartNew(RenderThread, canceller.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }
        }
    }
}
