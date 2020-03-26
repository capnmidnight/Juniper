using System;
using System.Threading;
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
        private IVeldridPanel panel;
        private Win32KeyEventSource keys;
        private Win32MouseMoveEventSource mouse;
        private VeldridDemoProgram demo;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (MainWindow is MainWindow wind)
            {
                window = wind;
                window.RequestStats += MainForm_RequestStats;

                panel = window.Panel;
                panel.Ready += Panel_Ready;
                panel.Destroying += Panel_Destroying;

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
                panel,
                canceller.Token);
            demo.Error += window.SetError;
            demo.Update += Demo_Update;
            keys.Start();
            mouse.Start();
            demo.Start();
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

        private void Panel_Destroying(object sender, EventArgs e)
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
