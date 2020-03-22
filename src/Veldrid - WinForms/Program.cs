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

    public static class Program
    {
        private static CancellationTokenSource canceller;
        private static MainWindow window;
        private static Win32KeyEventSource keys;
        private static Win32MouseMoveEventSource mouse;
        private static VeldridDemoProgram demo;

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = window = new MainWindow();
            window.Activated += MainForm_Activated;
            window.FormClosing += MainForm_FormClosing;
            window.RequestStats += MainForm_RequestStats;
            window.Panel.Resize += Panel_Resize;

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

            Application.Run(window);
        }

        private static uint Height => (uint)window.Panel.ClientSize.Height;

        private static uint Width => (uint)window.Panel.ClientSize.Width;

        private static void MainForm_Activated(object sender, EventArgs e)
        {
            window.Activated -= MainForm_Activated;
            _ = Task.Run(StartAsync);
        }

        private static Task StartAsync()
        {
            window.Panel.StopOwnRender();
            keys.Start();
            mouse.Start();
            return demo.StartAsync(
                Path.Combine("Shaders", "tex-cube.vert"),
                Path.Combine("Shaders", "tex-cube.frag"),
                Path.Combine("Models", "cube.obj"));
        }

        private static void Panel_Resize(object sender, EventArgs e)
        {
            demo.Resize(Width, Height);
        }

        private static void Demo_Update(float dt)
        {
            demo.SetVelocity(keys.GetAxis("horizontal"), keys.GetAxis("forward"));
        }

        private static void Mouse_Moved(object sender, MouseMovedEventArgs e)
        {
            demo.SetMouseRotate(e.DX, e.DY);
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

        private static void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            canceller.Cancel();
            mouse.Quit();
            keys.Quit();
            demo.Quit();
            demo.Dispose();
        }
    }
}
