using System;
using System.Numerics;
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
        private const float MOVE_SPEED = 1.5f;

        private static MainForm mainForm;
        private static Win32KeyEventSource keys;
        private static GraphicsDevice device;
        private static Swapchain swapchain;
        private static CommandList commandList;
        private static ShaderProgram<VertexPositionTexture> program;
        private static Camera camera;
        private static Vector2 lastMouse;
        private static CancellationTokenSource canceller;
        private static Task renderThread;

        private static float AspectRatio => (float)swapchain.Framebuffer.Width / swapchain.Framebuffer.Height;

        private static bool render;
        private static bool moving;
        private static Vector3 velocity;

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = mainForm = new MainForm();
            mainForm.Activated += MainForm_Activated;
            mainForm.FormClosing += MainForm_FormClosing;

            keys = new Win32KeyEventSource();
            keys.KeyChanged += Keys_KeyChanged;
            keys.AddKeyAlias("up", Keys.Up);
            keys.AddKeyAlias("down", Keys.Down);
            keys.AddKeyAlias("left", Keys.Left);
            keys.AddKeyAlias("right", Keys.Right);
            keys.DefineAxis("horizontal", "left", "right");
            keys.DefineAxis("forward", "up", "down");
            keys.Start();

            Application.Run(mainForm);
        }

        private static void MainForm_Activated(object sender, EventArgs e)
        {
            _ = Task.Run(StartAsync);
        }

        private static async Task StartAsync()
        {
            var options = mainForm.Device.Options;
            if (!GraphicsDevice.IsBackendSupported(mainForm.Device.Backend))
            {
                throw new NotSupportedException($"Graphics backend {mainForm.Device.Backend} is not supported on this system.");
            }

            device = mainForm.Device.Backend switch
            {
                GraphicsBackend.Direct3D11 => GraphicsDevice.CreateD3D11(options),
                GraphicsBackend.Metal => GraphicsDevice.CreateMetal(options),
                GraphicsBackend.Vulkan => GraphicsDevice.CreateVulkan(options),
                _ => null
            };

            if (device is null)
            {
                throw new InvalidOperationException($"Can't create a device for GraphicsBackend value: {mainForm.Device.Backend}");
            }

            var swapchainDescription = new SwapchainDescription
            {
                Source = mainForm.Panel.VeldridSwapchainSource,
                Width = (uint)mainForm.Panel.ClientSize.Width,
                Height = (uint)mainForm.Panel.ClientSize.Height,
                DepthFormat = options.SwapchainDepthFormat,
                SyncToVerticalBlank = options.SyncToVerticalBlank,
                ColorSrgb = options.SwapchainSrgbFormat
            };

            var resourceFactory = device.ResourceFactory;
            swapchain = resourceFactory.CreateSwapchain(swapchainDescription);
            commandList = device.ResourceFactory.CreateCommandList();

            var cubeProgramDescription = await ShaderProgramDescription.LoadAsync<VertexPositionTexture>(
                    "Shaders\\tex-cube-vert.glsl",
                    "Shaders\\tex-cube-frag.glsl")
                .ConfigureAwait(true);
            program = new ShaderProgram<VertexPositionTexture>(cubeProgramDescription, Mesh.ConvertVeldridMesh);
            program.LoadOBJ("Models/cube.obj");
            program.Begin(device, swapchain.Framebuffer, "ProjectionBuffer", "ViewBuffer", "WorldBuffer");

            program.Camera = camera = new Camera();
            camera.Position = 2.5f * Vector3.UnitZ;
            camera.Forward = -camera.Position;
            camera.AspectRatio = AspectRatio;

            mainForm.Panel.MouseMove += Panel_MouseMove;
            mainForm.Panel.Resize += Panel_Resize;
            mainForm.Panel.StopOwnRender();

            canceller = new CancellationTokenSource();
            renderThread = Task.Factory.StartNew(RenderThread, canceller.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private static void Keys_KeyChanged(object sender, KeyChangeEvent e)
        {
            var dx = keys.GetAxis("horizontal");
            var dz = keys.GetAxis("forward");
            moving = dx != 0 || dz != 0;
            if (moving)
            {
                velocity = MOVE_SPEED * Vector3.Transform(Vector3.Normalize(new Vector3(dx, 0, dz)), camera.Rotation);
            }
        }

        private static void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            var mouse = new Vector2(
                e.X,
                e.Y);
            if (lastMouse == Vector2.Zero)
            {
                lastMouse = mouse;
            }
            var delta = lastMouse - mouse;
            lastMouse = mouse;
            var dRot = Quaternion.CreateFromYawPitchRoll(
                Units.Degrees.Radians(delta.X),
                Units.Degrees.Radians(delta.Y),
                0);

            camera.Rotation *= dRot;
        }

        private static void Panel_Resize(object sender, EventArgs e)
        {
            render = false;
            swapchain.Resize((uint)mainForm.Panel.ClientSize.Width, (uint)mainForm.Panel.ClientSize.Width);
            camera.AspectRatio = AspectRatio;
            render = true;
        }


        private static void RenderThread()
        {
            GC.Collect();
            render = true;
            var start = DateTime.Now;
            var last = start;
            while (!canceller.IsCancellationRequested)
            {
                if (render)
                {
                    var time = (float)(DateTime.Now - start).TotalSeconds;
                    var dtime = (float)(DateTime.Now - last).TotalSeconds;
                    last = DateTime.Now;

                    if (moving)
                    {
                        camera.Position += velocity * dtime;
                    }

                    var worldMatrix = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, time);

                    commandList.Begin();
                    commandList.SetFramebuffer(swapchain.Framebuffer);

                    camera.Clear(commandList);

                    program.Draw(commandList, ref worldMatrix);

                    commandList.End();
                    device.SubmitCommands(commandList);
                    device.SwapBuffers(swapchain);
                    device.WaitForIdle();
                }
            }
        }

        private static void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (render)
            {
                render = false;
                keys.Quit();
                canceller.Cancel();
                _ = Task.Run(StopRenderingAsync);
            }
        }

        private static async Task StopRenderingAsync()
        {
            while (renderThread.IsRunning())
            {
                await Task.Yield();
            }

            program?.Dispose();
            commandList?.Dispose();
            swapchain?.Dispose();
            device?.Dispose();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }
    }
}
