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
        private static Matrix4x4 worldMatrix = Matrix4x4.Identity;
        private static CancellationTokenSource canceller;
        private static Thread updateThread;
        private static Thread renderThread;

        private static float AspectRatio => (float)swapchain.Framebuffer.Width / swapchain.Framebuffer.Height;

        private static bool running;

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = mainForm = new MainForm();
            mainForm.Activated += MainForm_Activated;

            canceller = new CancellationTokenSource();

            keys = new Win32KeyEventSource(canceller.Token);
            keys.AddKeyAlias("up", Keys.Up);
            keys.AddKeyAlias("down", Keys.Down);
            keys.AddKeyAlias("left", Keys.Left);
            keys.AddKeyAlias("right", Keys.Right);
            keys.DefineAxis("horizontal", "left", "right");
            keys.DefineAxis("forward", "up", "down");

            Application.Run(mainForm);

            canceller.Cancel();
            keys.Join();

            renderThread?.Join();
            updateThread?.Join();
            rendering?.Dispose();
            program?.Dispose();
            commandList?.Dispose();
            swapchain?.Dispose();
            device?.Dispose();
        }

        private static void MainForm_Activated(object sender, EventArgs e)
        {
            mainForm.Activated -= MainForm_Activated;
            keys.Start();
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

            camera = new Camera
            {
                AspectRatio = AspectRatio,
                Position = -2.5f * Vector3.UnitZ,
                Forward = Vector3.UnitZ
            };

            mainForm.Panel.MouseMove += Panel_MouseMove;
            mainForm.Panel.Resize += Panel_Resize;
            mainForm.Panel.StopOwnRender();

            GC.Collect();
            running = true;
            updateThread = new Thread(Update);
            renderThread = new Thread(Draw);
            renderThread.Start();
            updateThread.Start();
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
            running = false;
            swapchain.Resize((uint)mainForm.Panel.ClientSize.Width, (uint)mainForm.Panel.ClientSize.Width);
            camera.AspectRatio = AspectRatio;
            running = true;
        }

        private static void Update()
        {
            var start = DateTime.Now;
            var last = start;
            try
            {
                while (!canceller.IsCancellationRequested)
                {
                    var time = (float)(DateTime.Now - start).TotalSeconds;
                    var dtime = (float)(DateTime.Now - last).TotalSeconds;
                    last = DateTime.Now;

                    var dx = keys.GetAxis("horizontal");
                    var dz = keys.GetAxis("forward");
                    var moving = dx != 0 || dz != 0;
                    if (moving)
                    {
                        var velocity = MOVE_SPEED * Vector3.Transform(Vector3.Normalize(new Vector3(dx, 0, dz)), camera.Rotation);
                        camera.Position += velocity * dtime;
                    }

                    worldMatrix = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, time);
                }
            }
            catch (Exception exp)
            {
                mainForm.SetError(exp);
            }
        }


        private static void Draw()
        {
            try
            {
                while (!canceller.IsCancellationRequested)
                {
                    if (running)
                    {
                        commandList.Begin();
                        commandList.SetFramebuffer(swapchain.Framebuffer);

                        camera.Clear(commandList);

                        program.Draw(commandList, camera, ref worldMatrix);

                        commandList.End();
                        device.SubmitCommands(commandList);
                        device.SwapBuffers(swapchain);
                        device.WaitForIdle();
                    }
                }
            }
            catch (Exception exp)
            {
                mainForm.SetError(exp);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }
    }
}
