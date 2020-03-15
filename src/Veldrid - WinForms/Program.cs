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
        const float MOVE_SPEED = 0.01f;
        private static Camera camera;
        private static ShaderProgram<VertexPositionColor> quadProgram;
        private static ShaderProgram<VertexPositionTexture> cubeProgram;
        private static MainForm mainForm;
        private static DateTime start;
        private static DateTime last;
        private static Win32KeyEventSource keys;
        private static Vector2 lastMouse;

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = mainForm = new MainForm();
            mainForm.Panel.Resize += Panel_Resize;
            mainForm.Panel.Ready += Panel_Ready;
            mainForm.Panel.CommandListUpdate += Panel_CommandListUpdate;
            mainForm.FormClosing += MainForm_FormClosing;
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

        private static void Panel_Ready(object sender, EventArgs e)
        {
            _ = Task.Run(() => PrepareShaderProgramAsync(mainForm.Device.VeldridDevice, mainForm.Panel.VeldridSwapChain.Framebuffer));
        }

        private static async Task PrepareShaderProgramAsync(GraphicsDevice device, Framebuffer framebuffer)
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

            var quad = new Mesh<VertexPositionColor>(
                new Quad<VertexPositionColor>(
                    new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
                    new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
                    new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
                    new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)));
            var quadProgramDescription = await ShaderProgramDescription.LoadAsync<VertexPositionColor>(
                    "Shaders\\color-quad-vert.glsl",
                    "Shaders\\color-quad-frag.glsl")
                .ConfigureAwait(false);
            quadProgram = new ShaderProgram<VertexPositionColor>(quadProgramDescription, quad);
            quadProgram.Begin(device, framebuffer);

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
                .ConfigureAwait(false);
            cubeProgram = new ShaderProgram<VertexPositionTexture>(cubeProgramDescription, cube);
            cubeProgram.AddTexture("SurfaceTexture", images["rock"]);
            cubeProgram.Begin(device, framebuffer);

            camera = cubeProgram.CreateCamera("ProjectionBuffer", "ViewBuffer");
            camera.Position = 2.5f * Vector3.UnitZ;
            camera.Forward = Vector3.Zero - camera.Position;
            camera.AspectRatio = mainForm.Panel.AspectRatio;

            last = start = DateTime.Now;
        }

        private static void Panel_Resize(object sender, EventArgs e)
        {
            if(camera is object)
            {
                camera.AspectRatio = mainForm.Panel.AspectRatio;
            }
        }

        private static void SetFPS(float fps)
        {
            mainForm.Text = $"MainForm - {fps:0.0}fps";
        }

        private static readonly Delegate setFPS = new Action<float>(SetFPS);

        private static void Panel_CommandListUpdate(object sender, UpdateCommandListEventArgs e)
        {
            if (quadProgram?.IsRunning == true
                && cubeProgram?.IsRunning == true
                && camera is object)
            {
                var commandList = e.CommandList;

                camera.AspectRatio = (float)e.Width / e.Height;

                var time = (float)(DateTime.Now - start).TotalSeconds;
                var dtime = (float)(DateTime.Now - last).TotalSeconds;
                var fps = Units.Seconds.Hertz(dtime);
                mainForm.Invoke(setFPS, fps);
                last = DateTime.Now;

                var showQuad = (((int)(time / 5)) % 2) == 0;

                commandList.ClearColorTarget(0, RgbaFloat.Black);
                commandList.ClearDepthStencil(1);

                if (false && showQuad)
                {
                    var size = Math.Min(e.Width, e.Height);

                    commandList.SetViewport(0, new Viewport(
                        x: (e.Width - size) / 2,
                        y: (e.Height - size) / 2,
                        width: size,
                        height: size,
                        minDepth: 0,
                        maxDepth: 1));

                    quadProgram.Draw(commandList);
                }
                else
                {
                    var dx = keys.GetAxis("horizontal");
                    var dz = keys.GetAxis("forward");
                    if (dx != 0 || dz != 0)
                    {
                        var move = Vector3.Normalize(new Vector3(dx, 0, dz));
                        camera.Position += Vector3.Transform(move, camera.Rotation) * time * MOVE_SPEED;
                    }
                    

                    camera.SetView(commandList);

                    var worldMatrix = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, time);
                    cubeProgram.UpdateMatrix("WorldBuffer", commandList, ref worldMatrix);
                    cubeProgram.Draw(commandList);
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }

        private static void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cubeProgram?.Dispose();
            cubeProgram = null;

            quadProgram?.Dispose();
            quadProgram = null;

            camera?.Dispose();
            camera = null;
        }
    }
}
