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
            mainForm.Panel.Resize += Panel_Resize;

            canceller = new CancellationTokenSource();

            demo = new VeldridDemoProgram(
                mainForm.Device.Backend,
                mainForm.Device.Options,
                mainForm.Panel.VeldridSwapchainSource,
                Width, Height,
                canceller.Token);
            demo.Error += mainForm.SetError;

            keys = new Win32KeyEventSource(canceller.Token);
            keys.Changed += Keys_Changed;
            keys.AddKeyAlias("up", Keys.Up);
            keys.AddKeyAlias("down", Keys.Down);
            keys.AddKeyAlias("left", Keys.Left);
            keys.AddKeyAlias("right", Keys.Right);
            keys.DefineAxis("horizontal", "left", "right");
            keys.DefineAxis("forward", "up", "down");
            keys.Start();

            mouse = new Win32MouseMoveEventSource(canceller.Token);
            mouse.Moved += Mouse_Moved;
            mouse.Start();

            Application.Run(mainForm);

            canceller.Cancel();
            mouse.Quit();
            keys.Quit();
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
            return demo.StartAsync(
                Path.Combine("Shaders", "tex-cube-vert.glsl"),
                Path.Combine("Shaders", "tex-cube-frag.glsl"),
                Path.Combine("Models", "cube.obj"));
        }

        private static void Keys_Changed(object sender, KeyChangeEventArgs e)
        {
            demo.SetVelocity(keys.GetAxis("horizontal"), keys.GetAxis("forward"));
        }

        private static void Mouse_Moved(object sender, MouseMovedEventArgs e)
        {
            demo.SetMouseRotate(e.DX, e.DY);
        }

        private static void Panel_Resize(object sender, EventArgs e)
        {
            demo.Resize(Width, Height);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }
    }
}
