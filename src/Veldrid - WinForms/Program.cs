using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.Imaging;
using Juniper.Input;
using Juniper.IO;
using Juniper.VeldridIntegration;

using Veldrid;

namespace Juniper
{
    public static class Program
    {
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

        private static event Action<float> UpdateFPS;

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = mainForm = new MainForm();
            mainForm.Activated += MainForm_Activated;
            mainForm.FormClosing += MainForm_FormClosing;
            mainForm.Panel.Resize += Panel_Resize;
            mainForm.Panel.MouseMove += Panel_MouseMove;

            keys = new Win32KeyEventSource();
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
            var images = GetImages();

            canceller = new CancellationTokenSource();

            var options = mainForm.Device.Options;
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

            var cube = new Mesh<VertexPositionTexture>(
                // Top
                new Quad<VertexPositionTexture>(
                    new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                    new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 1))),
                // Bottom
                new Quad<VertexPositionTexture>(
                    new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(1, 0)),
                    new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(1, 1))),
                // Left
                new Quad<VertexPositionTexture>(
                    new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
                    new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(1, 1))),
                // Right
                new Quad<VertexPositionTexture>(
                    new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                    new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(1, 1))),
                // Back
                new Quad<VertexPositionTexture>(
                    new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                    new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1, 1))),
                // Front
                new Quad<VertexPositionTexture>(
                    new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
                    new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(1, 1))));
            var cubeProgramDescription = await ShaderProgramDescription.LoadAsync<VertexPositionTexture>(
                    "Shaders\\tex-cube-vert.glsl",
                    "Shaders\\tex-cube-frag.glsl")
                .ConfigureAwait(true);
            program = new ShaderProgram<VertexPositionTexture>(cubeProgramDescription, cube);
            program.AddTexture("SurfaceTexture", images["rock"]);
            program.Begin(device, swapchain.Framebuffer);

            camera = program.CreateCamera("ProjectionBuffer", "ViewBuffer");
            camera.Position = 2.5f * Vector3.UnitZ;
            camera.Forward = Vector3.Zero - camera.Position;
            camera.AspectRatio = AspectRatio;

            mainForm.Panel.StopOwnRender();
            renderThread = Task.Factory.StartNew(RenderThread, canceller.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private static Dictionary<string, ImageData> GetImages()
        {
            var imageDir = new DirectoryInfo("Images");
            if (!imageDir.Exists)
            {
                throw new FileNotFoundException("Could not find the Images directory at " + imageDir.FullName);
            }

            var images = new Dictionary<string, ImageData>();
            var decoders = new Dictionary<MediaType, IImageCodec<ImageData>>()
            {
                [MediaType.Image.Png] = new HjgPngcsCodec().Pipe(new HjgPngcsImageDataTranscoder()),
                [MediaType.Image.Jpeg] = new LibJpegNETCodec().Pipe(new LibJpegNETImageDataTranscoder(padAlpha: true))
            };

            foreach (var file in imageDir.EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name)
                    .ToLowerInvariant();
                var type = (from t in MediaType.GuessByFile(file)
                            where t is MediaType.Image
                            select t)
                        .FirstOrDefault();
                if (decoders.ContainsKey(type))
                {
                    images[name] = decoders[type].Deserialize(file);
                }
            }

            return images;
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

            camera?.Dispose();
            program?.Dispose();
            commandList?.Dispose();
            swapchain?.Dispose();
            device?.Dispose();
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
            if (camera != null)
            {
                var dRot = Quaternion.CreateFromYawPitchRoll(
                    Units.Degrees.Radians(delta.X),
                    Units.Degrees.Radians(delta.Y),
                    0);

                camera.Rotation *= dRot;
            }
        }

        private static void Panel_Resize(object sender, EventArgs e)
        {
            render = false;
            swapchain?.Resize((uint)mainForm.Panel.ClientSize.Width, (uint)mainForm.Panel.ClientSize.Width);
            if (camera is object)
            {
                camera.AspectRatio = AspectRatio;
            }
            render = true;
        }


        private static void RenderThread()
        {
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

                    var dx = keys.GetAxis("horizontal");
                    var dz = keys.GetAxis("forward");
                    if (dx != 0 || dz != 0)
                    {
                        var move = Vector3.Normalize(new Vector3(dx, 0, dz));
                        camera.Position += Vector3.Transform(move, camera.Rotation) * dtime;
                    }

                    var worldMatrix = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, time);

                    commandList.Begin();
                    commandList.SetFramebuffer(swapchain.Framebuffer);
                    commandList.ClearColorTarget(0, RgbaFloat.Black);
                    commandList.ClearDepthStencil(1);

                    camera.SetView(commandList);

                    program.UpdateMatrix("WorldBuffer", commandList, ref worldMatrix);
                    program.Draw(commandList);
                    commandList.End();
                    device.SubmitCommands(commandList);
                    device.SwapBuffers(swapchain);
                    device.WaitForIdle();
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }
    }
}
