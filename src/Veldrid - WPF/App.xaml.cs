using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.Input;
using Juniper.VeldridIntegration;

using Veldrid;

namespace Juniper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : System.Windows.Application,
        IDisposable
    {
        private CancellationTokenSource canceller;
        private MainWindow window;
        private Win32KeyEventSource keys;
        private Win32MouseMoveEventSource mouse;
        private VeldridDemoProgram demo;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (MainWindow is MainWindow wind)
            {
                window = wind;
                window.Closing += Window_Closing;
                window.RequestStats += MainForm_RequestStats;
                window.Panel.Resize += Panel_Resize;
                window.Panel.Ready += Panel_Ready;

                canceller = new CancellationTokenSource();

                keys = new Win32KeyEventSource(canceller.Token);
                keys.AddKeyAlias("up", Keys.Up);
                keys.AddKeyAlias("down", Keys.Down);
                keys.AddKeyAlias("left", Keys.Left);
                keys.AddKeyAlias("right", Keys.Right);
                keys.DefineAxis("horizontal", "left", "right");
                keys.DefineAxis("forward", "up", "down");

                mouse = new Win32MouseMoveEventSource(canceller.Token);
                mouse.Moved += Mouse_Moved;
            }
        }

        private void Panel_Ready(object sender, EventArgs e)
        {
            demo = new VeldridDemoProgram(
                GraphicsBackend.Vulkan,
                new GraphicsDeviceOptions
                {
                    PreferDepthRangeZeroToOne = true,
                    PreferStandardClipSpaceYDirection = true,
                    ResourceBindingModel = ResourceBindingModel.Improved,
                    SwapchainDepthFormat = (PixelFormat)SwapchainDepthFormat.R16_UNorm,
                    SwapchainSrgbFormat = false,
                    SyncToVerticalBlank = true,
                    HasMainSwapchain = false
                },
                window.Panel.VeldridSwapchainSource,
                Width, Height,
                canceller.Token);
            demo.Error += window.SetError;
            demo.Update += Demo_Update;
            _ = Task.Run(StartAsync);
        }

        private uint Height => (uint)window.Panel.RenderSize.Height;

        private uint Width => (uint)window.Panel.RenderSize.Width;

        private Task StartAsync()
        {
            keys.Start();
            mouse.Start();
            return demo.StartAsync(
                Path.Combine("Shaders", "tex-cube.vert"),
                Path.Combine("Shaders", "tex-cube.frag"),
                Path.Combine("Models", "cube.obj"));
        }

        private void Panel_Resize(object sender, EventArgs e)
        {
            demo.Resize(Width, Height);
        }

        private void Demo_Update(float dt)
        {
            demo.SetVelocity(keys.GetAxis("horizontal"), keys.GetAxis("forward"));
        }

        private void Mouse_Moved(object sender, MouseMovedEventArgs e)
        {
            demo.SetMouseRotate(e.DX, e.DY);
        }

        private void MainForm_RequestStats(object sender, EventArgs e)
        {
            if (window is object
                && demo is object)
            {
                window.SetStats(
                    demo.MinFramesPerSecond,
                    demo.MeanFramesPerSecond,
                    demo.MaxFramesPerSecond);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            canceller.Cancel();
            mouse.Quit();
            keys.Quit();
            demo.Quit();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                demo.Dispose();
                canceller.Dispose();
            }
        }
    }
}
