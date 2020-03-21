using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.Input;

namespace Juniper
{

    public static class Program
    {
        private static CancellationTokenSource canceller;
        private static MainForm mainForm;
        private static Win32KeyEventSource keys;
        private static Win32MouseMoveEventSource mouse;
        private static VeldridDemoProgram demo;

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = mainForm = new MainForm();
            mainForm.Activated += MainForm_Activated;
            mainForm.FormClosing += MainForm_FormClosing;
            mainForm.RequestStats += MainForm_RequestStats;
            mainForm.Panel.Resize += Panel_Resize;

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
                mainForm.Device.Backend,
                mainForm.Device.Options,
                mainForm.Panel.VeldridSwapchainSource,
                Width, Height,
                canceller.Token);
            demo.Error += mainForm.SetError;
            demo.Update += Demo_Update;

            Application.Run(mainForm);

            demo.Dispose();
        }

        private static uint Height => (uint)mainForm.Panel.ClientSize.Height;

        private static uint Width => (uint)mainForm.Panel.ClientSize.Width;

        private static void MainForm_Activated(object sender, EventArgs e)
        {
            mainForm.Activated -= MainForm_Activated;
            _ = Task.Run(StartAsync);
        }

        private static Task StartAsync()
        {
            mainForm.Panel.StopOwnRender();
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
            mainForm.SetStats(
                demo.MinFramesPerSecond,
                demo.MeanFramesPerSecond,
                demo.MaxFramesPerSecond);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }

        private static void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            canceller.Cancel();
            mouse.Quit();
            keys.Quit();
            demo.Quit();
        }
    }
}
