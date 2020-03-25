using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.Input;

using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Juniper
{

    public static class Program
    {
        private static CancellationTokenSource canceller;
        private static Sdl2Window window;
        private static Win32KeyEventSource keys;
        private static Win32MouseMoveEventSource mouse;
        private static VeldridDemoProgram demo;

        private static void Main()
        {
            try
            {
                canceller = new CancellationTokenSource();

                window = new Sdl2Window(
                    "Veldrid - Console",
                    100, 100,
                    1280, 720,
                    SDL_WindowFlags.AllowHighDpi | SDL_WindowFlags.OpenGL | SDL_WindowFlags.Shown,
                    true);

                using var device = VeldridStartup.CreateGraphicsDevice(
                    window,
                    new GraphicsDeviceOptions
                    {
                        SwapchainDepthFormat = PixelFormat.D24_UNorm_S8_UInt,
                        ResourceBindingModel = ResourceBindingModel.Improved,
                        SwapchainSrgbFormat = false,
                        SyncToVerticalBlank = true,
                        PreferDepthRangeZeroToOne = true,
                        PreferStandardClipSpaceYDirection = true,
                    },
                    GraphicsBackend.Vulkan);

                keys = new Win32KeyEventSource(canceller.Token);
                keys.AddKeyAlias("up", Keys.Up);
                keys.AddKeyAlias("down", Keys.Down);
                keys.AddKeyAlias("left", Keys.Left);
                keys.AddKeyAlias("right", Keys.Right);
                keys.AddKeyAlias("quit", Keys.Escape);
                keys.DefineAxis("horizontal", "left", "right");
                keys.DefineAxis("forward", "up", "down");

                mouse = new Win32MouseMoveEventSource(canceller.Token);
                mouse.Moved += Mouse_Moved;

                demo = new VeldridDemoProgram(
                    device,
                    canceller.Token);
                demo.Error += Demo_Error; ;
                demo.Update += Demo_Update;

                keys.Start();
                mouse.Start();
                demo.Start();

                while (!canceller.IsCancellationRequested)
                {
                    _ = window.PumpEvents();
                }

                mouse.Quit();
                keys.Quit();
                demo.Quit();
                demo.Dispose();
                window.Close();
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine(exp.Unroll());
            }
        }

        private static void Demo_Error(Exception obj)
        {
            Console.Error.WriteLine(obj.Unroll());
        }

        private static void Demo_Update(float dt)
        {
            if (keys.IsDown("quit"))
            {
                canceller.Cancel();
            }

            demo.SetVelocity(keys.GetAxis("horizontal"), keys.GetAxis("forward"));
        }

        private static void Mouse_Moved(object sender, MouseMovedEventArgs e)
        {
            demo.SetMouseRotate(e.DX, e.DY);
        }
    }
}
