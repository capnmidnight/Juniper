using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.Input;
using Juniper.VeldridIntegration;

using Veldrid;

namespace Juniper
{

    public static class Program
    {
        private static CancellationTokenSource canceller;
        private static MainWindow window;
        private static IVeldridPanel panel;
        private static VeldridDemoProgram demo;

        private static Win32KeyEventSource keys;
        private static Win32MouseMoveEventSource mouse;

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = window = new MainWindow();
            panel = form.Panel;
            panel.Ready += Panel_Ready;
            panel.Destroying += Panel_Destroying;
            //window.RequestStats += MainForm_RequestStats;

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

            Application.Run(form);
        }

        private static void Panel_Ready(object sender, EventArgs e)
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

            _ = Task.Run(demo.StartAsync)
                .OnError(window.SetError);
        }

        private static void Demo_Update(float dt)
        {
            demo.SetVelocity(keys.GetAxis("horizontal"), keys.GetAxis("forward"));
        }

        private static void Mouse_Moved(object sender, MouseMovedEventArgs e)
        {
            demo.SetMouseRotate(e.DX, e.DY);
        }

        private static void Panel_Destroying(object sender, EventArgs e)
        {
            canceller.Cancel();
            mouse.Quit();
            keys.Quit();
            demo.Quit();
            demo.Dispose();
        }

        private static void MainForm_RequestStats(object sender, EventArgs e)
        {
            window.SetStats(
                demo.MinFramesPerSecond,
                demo.MeanFramesPerSecond,
                demo.MaxFramesPerSecond);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            window.SetError(e.Exception);
        }
    }
}
