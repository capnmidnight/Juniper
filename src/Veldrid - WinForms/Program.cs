using System;
using System.Numerics;
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
        private static Vector2 lastMouse;
        private static VeldridDemoProgram demo;

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = mainForm = new MainForm();
            mainForm.Activated += MainForm_Activated;
            mainForm.Panel.Resize += Panel_Resize;
            mainForm.Panel.MouseMove += Panel_MouseMoveStart;

            canceller = new CancellationTokenSource();

            keys = new Win32KeyEventSource(canceller.Token);
            keys.KeyChanged += Keys_KeyChanged;
            keys.AddKeyAlias("up", Keys.Up);
            keys.AddKeyAlias("down", Keys.Down);
            keys.AddKeyAlias("left", Keys.Left);
            keys.AddKeyAlias("right", Keys.Right);
            keys.DefineAxis("horizontal", "left", "right");
            keys.DefineAxis("forward", "up", "down");
            keys.Start();

            Application.Run(mainForm);

            canceller.Cancel();
            keys.Join();

            demo?.Dispose();
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
            demo = new VeldridDemoProgram(
                mainForm.Device.Backend,
                mainForm.Device.Options,
                mainForm.Panel.VeldridSwapchainSource,
                Width, Height,
                canceller.Token);

            demo.Error += mainForm.SetError;
            mainForm.Panel.StopOwnRender();
            return demo.StartAsync(
                "Shaders\\tex-cube-vert.glsl",
                "Shaders\\tex-cube-frag.glsl");
        }

        private static void Keys_KeyChanged(object sender, KeyChangeEvent e)
        {
            demo?.SetVelocity(keys.GetAxis("horizontal"), keys.GetAxis("forward"));
        }

        private static void Panel_MouseMoveStart(object sender, MouseEventArgs e)
        {
            lastMouse = new Vector2(e.X, e.Y);
            mainForm.Panel.MouseMove -= Panel_MouseMoveStart;
            mainForm.Panel.MouseMove += Panel_MouseMoveContinue;
        }

        private static void Panel_MouseMoveContinue(object sender, MouseEventArgs e)
        {
            var mouse = new Vector2(e.X, e.Y);
            var delta = lastMouse - mouse;
            lastMouse = mouse;
            demo?.SetMouseRotate(delta);
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
